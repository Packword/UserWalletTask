namespace UserWallet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly ExchangeRateGenerator _exchangeRateGenerator;
        private readonly IConvertToUsdService _convertToUsdService;
        private readonly IDepositFiatService _depositFiatService;
        private readonly IDepositCryptoService _depositCryptoService;
        private readonly ITransactionService _transactionService;
        private readonly IUserBalanceService _userBalanceService;

        private readonly List<Currency> currencies;
        private readonly HashSet<string> availableCurrencies = new();

        public WalletController(ExchangeRateGenerator exchangeRateGenerator,
                                IConvertToUsdService convertToUsdService,
                                IDepositFiatService depositFiatService,
                                IDepositCryptoService depositCryptoService,
                                ITransactionService transactionService,
                                IUserBalanceService userBalanceService)
        {
            _exchangeRateGenerator = exchangeRateGenerator;
            _convertToUsdService = convertToUsdService;
            _depositFiatService = depositFiatService;
            _depositCryptoService = depositCryptoService;
            _transactionService = transactionService;
            _userBalanceService = userBalanceService;

            currencies = _exchangeRateGenerator.GetCurrencies();
            availableCurrencies = currencies.Where(c => c.IsAvailable).Select(c => c.Id).ToHashSet();
        }

        [HttpGet("balance")]
        [Authorize(Roles = UsersRole.USER)]
        public Dictionary<string, BalanceDTO>? GetCurrentUserBalances()
        {
            int id = HttpContext.GetCurrentUserId()!.Value;
            var balances = _userBalanceService.GetUserBalances(id);
            return ConvertToBalanceDTO(balances);
        }

        [HttpGet("tx")]
        [Authorize(Roles = UsersRole.USER)]
        public List<Deposit>? GetUserTransactions()
            => _transactionService.GetUserDeposits(HttpContext.GetCurrentUserId()!.Value);

        [HttpGet("{id:int}")]
        [Authorize(Roles = UsersRole.ADMIN)]
        public Dictionary<string, BalanceDTO>? GetUserBalanceInUsdById(int id)
        {
            var balances = _userBalanceService.GetUserBalances(id);
            return ConvertToBalanceDTO(balances);
        }

        [HttpPut("deposit/{currencyId}")]
        [Authorize(Roles = UsersRole.USER)]
        public IActionResult CreateDeposit(string currencyId, [FromBody] DepositDTO depositDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.SelectMany(v => v.Errors));

            if (!availableCurrencies.Contains(currencyId))
                return BadRequest("Unavailable currency");

            Currency currency = currencies.First(c => c.Id == currencyId);
            int userId = HttpContext.GetCurrentUserId()!.Value;
            var result = currency.Type switch
            {
                CurrencyType.Fiat => _depositFiatService.CreateDeposit(userId, depositDTO, currency.Id),
                CurrencyType.Crypto => _depositCryptoService.CreateDeposit(userId, depositDTO, currency.Id),
                _ => false,
            };

            if (!result)
                return BadRequest("Wrong additional data lenght, crypto address and fiat cardnumber must be 16 characters, and fiat cardholder name between 2 and 16 characters");

            return Ok();
        }

        private Dictionary<string, BalanceDTO> ConvertToBalanceDTO(List<UserBalance>? balances)
        {
            if (balances is null)
                return new();

            var usdBalances = _convertToUsdService.ConvertCurrency(balances.Select(x => (x.CurrencyId, x.Amount)).ToList());
            var balancesZip = usdBalances.Zip(balances);

            return balancesZip.ToDictionary(
                    key => key.First.CurrencyId,
                    value => new BalanceDTO
                    {
                        Amount = value.Second.Amount,
                        UsdAmount = value.First.UsdAmount
                    }
                );
        }
    }
}

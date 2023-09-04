namespace UserWallet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;
        private readonly IDepositFiatService _depositFiatService;
        private readonly IDepositCryptoService _depositCryptoService;
        private readonly ITransactionService _transactionService;
        private readonly IUserBalanceService _userBalanceService;

        private readonly List<Currency>? currencies;
        private readonly HashSet<string>? availableCurrencies;

        public WalletController(ICurrencyService currencyService,
                                IDepositFiatService depositFiatService,
                                IDepositCryptoService depositCryptoService,
                                ITransactionService transactionService,
                                IUserBalanceService userBalanceService)
        {
            _currencyService = currencyService;
            _depositFiatService = depositFiatService;
            _depositCryptoService = depositCryptoService;
            _transactionService = transactionService;
            _userBalanceService = userBalanceService;

            currencies = _currencyService.GetCurrencies();
            availableCurrencies = currencies?.Where(c => c.IsAvailable).Select(c => c.Id).ToHashSet();
        }

        [HttpGet("balance")]
        [Authorize(Roles = UsersRole.USER)]
        public IActionResult GetCurrentUserBalances()
        {
            int id = HttpContext.GetCurrentUserId()!.Value;
            (bool Result, List<UserBalance>? Balances) = _userBalanceService.GetUserBalances(id);
            if(!Result)
                return NotFound();
            return Ok(_userBalanceService.ConvertToBalanceDTO(Balances));
        }

        [HttpGet("tx")]
        [Authorize(Roles = UsersRole.USER)]
        public List<Deposit>? GetUserTransactions()
            => _transactionService.GetUserDeposits(HttpContext.GetCurrentUserId()!.Value);

        [HttpGet("{id:int}")]
        [Authorize(Roles = UsersRole.ADMIN)]
        public IActionResult GetUserBalanceInUsdById(int id)
        {
            var (Result, Balances) = _userBalanceService.GetUserBalances(id);
            if (!Result)
                return NotFound();

            return Ok(_userBalanceService.ConvertToBalanceDTO(Balances));
        }

        [HttpPut("deposit/{currencyId}")]
        [Authorize(Roles = UsersRole.USER)]
        public IActionResult CreateDeposit(string currencyId, [FromBody] DepositDTO depositDTO)
        {
            if (!availableCurrencies.Contains(currencyId))
                return BadRequest("Unavailable currency");

            Currency currency = currencies.First(c => c.Id == currencyId);
            int userId = HttpContext.GetCurrentUserId()!.Value;
            (bool Result, string Message) = currency.Type switch
            {
                CurrencyType.Fiat => _depositFiatService.CreateDeposit(userId, depositDTO, currency.Id),
                CurrencyType.Crypto => _depositCryptoService.CreateDeposit(userId, depositDTO, currency.Id),
                _ => (false, "Unknown type")
            };

            if (!Result)
                return BadRequest(Message);

            return Ok();
        }

        
    }
}

using UserWallet.Services.Extensions;

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
        private HashSet<string> availableCurrencies = new HashSet<string>();

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
            int id = HttpContext.GetCurrentUserId();
            var balances = _userBalanceService.GetUserBalances(id);
            return ConvertBalanceToDictionaryWithUsd(balances);
        }

        [HttpGet("tx")]
        [Authorize(Roles = UsersRole.USER)]
        public List<Deposit>? GetUserTransactions()
            => _transactionService.GetUserDeposits(HttpContext.GetCurrentUserId());

        [HttpGet("{id:int}")]
        [Authorize(Roles = UsersRole.ADMIN)]
        public Dictionary<string, BalanceDTO>? GetUserBalanceInUsdById(int id)
        {
            var balances = _userBalanceService.GetUserBalances(id);
            return ConvertBalanceToDictionaryWithUsd(balances);
        }

        [HttpPut("deposit/{currency}")]
        [Authorize(Roles = UsersRole.USER)]
        public IActionResult CreateDeposit(string currency, [FromBody] DepositDTO depositDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.SelectMany(v => v.Errors));

            if (!availableCurrencies.Contains(currency))
                return BadRequest("Unavailable currency");

            Currency curr = currencies.First(c => c.Id == currency);
            int userId = HttpContext.GetCurrentUserId();

            bool result;
            switch (curr.Type)
            {
                case CurrencyType.Fiat:
                    result = _depositFiatService.CreateDeposit(depositDTO, userId, curr.Id);
                    break;
                case CurrencyType.Crypto:
                    result = _depositCryptoService.CreateDeposit(depositDTO, userId, curr.Id);
                    break;
                default:
                    result = false;
                    break;
            }

            if (!result)
                return BadRequest("Invalid additional data");

            return Ok();
        }

        private Dictionary<string, BalanceDTO>? ConvertBalanceToDictionaryWithUsd(List<UserBalance> balances)
        {
            return balances?.ToDictionary(key => key.CurrencyId,
                                          value => new BalanceDTO
                                          {
                                              Amount = value.Amount,
                                              UsdAmount = _convertToUsdService.ConvertCurrency(value.CurrencyId, value.Amount)
                                          });
        }
    }
}

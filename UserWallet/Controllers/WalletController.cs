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
        private readonly IHttpContextService _httpContextService;

        private readonly List<Currency> currencies;
        private HashSet<string> availableCurrencies = new HashSet<string>();

        public WalletController(ExchangeRateGenerator exchangeRateGenerator, 
                                IUserService userService,
                                IConvertToUsdService convertToUsdService,
                                IDepositFiatService depositFiatService,
                                IDepositCryptoService depositCryptoService,
                                ITransactionService transactionService,
                                IHttpContextService httpContextService)
        {
            _exchangeRateGenerator = exchangeRateGenerator;
            _convertToUsdService = convertToUsdService;
            _depositFiatService = depositFiatService;
            _depositCryptoService = depositCryptoService;
            _transactionService = transactionService;
            _httpContextService = httpContextService;

            currencies = _exchangeRateGenerator.GetCurrencies();
            availableCurrencies = currencies.Where(c => c.IsAvailable).Select(c => c.Id).ToHashSet();
        }

        [HttpGet("balance")]
        [Authorize(Roles = "User")]
        public Dictionary<string, BalanceDTO>? GetCurrentUserBalances()
            => _convertToUsdService.GenerateUserBalance(_httpContextService.GetCurrentUserId(HttpContext));

        [HttpGet("tx")]
        [Authorize(Roles = "User")]
        public List<Deposit>? GetUserTransactions()
            => _transactionService.GetUserDeposits(_httpContextService.GetCurrentUserId(HttpContext));

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        public Dictionary<string, BalanceDTO>? GetUserBalanceInUsdById(int id)
            => _convertToUsdService.GenerateUserBalance(id);

        [HttpPut("deposit/{currency}")]
        [Authorize(Roles = "User")]
        public IActionResult CreateDeposit(string currency, [FromBody] DepositDTO depositDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (!availableCurrencies.Contains(currency))
                return BadRequest();

            Currency curr = currencies.First(c => Equals(c.Id, currency));
            int userId = _httpContextService.GetCurrentUserId(HttpContext);

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
                return BadRequest();

            return Ok();
        }
    }
}

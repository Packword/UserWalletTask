using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace UserWallet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly ExchangeRateGenerator _exchangeRateGenerator;
        private readonly IUserService _userService;
        private readonly IConvertToUsdService _convertToUsdService;
        private readonly IDepositFiatService _depositFiatService;
        private readonly IDepositCryptoService _depositCryptoService;
        private readonly ITransactionService _transactionService;

        private readonly List<Currency>? currencies;
        private HashSet<string> availableCurrencies = new HashSet<string>();

        public WalletController(ExchangeRateGenerator exchangeRateGenerator, 
                                IUserService userService,
                                IConvertToUsdService convertToUsdService,
                                IDepositFiatService depositFiatService,
                                IDepositCryptoService depositCryptoService,
                                ITransactionService transactionService)
        {
            _exchangeRateGenerator = exchangeRateGenerator;
            _userService = userService;
            _convertToUsdService = convertToUsdService;
            _depositFiatService = depositFiatService;
            _depositCryptoService = depositCryptoService;
            _transactionService = transactionService;
            currencies = _exchangeRateGenerator.GetCurrencies();
            foreach (var currency in currencies)
            {
                if (currency.IsAvailable)
                    availableCurrencies.Add(currency.Id);
            }
        }

        [HttpGet("balance")]
        [Authorize(Roles = "User")]
        public Dictionary<string, BalanceDTO>? GetCurrentUserBalances()
        {
            User? user = _userService.GetUserById(GetCurrentUserId());
            Dictionary<string, BalanceDTO>? result = _convertToUsdService.GenerateUserBalance(user);
            return result;
        }

        [HttpGet("tx")]
        [Authorize(Roles = "User")]
        public List<Deposit>? GetUserTransactions()
        {
            int id = GetCurrentUserId();
            return _transactionService.GetUserDeposits(id);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetUserBalanceById(int id)
        {
            User? user = _userService.GetUserById(id);
            if (user is null)
                return NotFound();
            Dictionary<string, BalanceDTO> result = _convertToUsdService.GenerateUserBalance(user);
            return Ok(result);
        }

        [HttpPut("deposit/{currency}")]
        [Authorize(Roles = "User")]
        public IActionResult CreateDeposit(string currency, [FromBody] DepositDTO depositDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (!availableCurrencies.Contains(currency))
                return BadRequest();

            Currency curr = currencies.First(c => Equals(c.Id, currency));
            bool result;
            switch (curr.Type)
            {
                case "Fiat":
                    result = _depositFiatService.CreateDeposit(depositDTO, GetCurrentUserId(), curr.Id);
                    break;
                case "Crypto":
                    result = _depositCryptoService.CreateDeposit(depositDTO, GetCurrentUserId(), curr.Id);
                    break;
                default:
                    result = false;
                    break;
            }
            if (!result)
                return BadRequest();

            return Ok();

        }

        private int GetCurrentUserId()
        {
            return int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserWallet.DTOs;
using UserWallet.Interfaces;
using UserWallet.Models;
using UserWallet.Services;

namespace UserWallet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly ExchangeRateGenerator _exchangeRateGenerator;
        private readonly IUserService _userService;
        private readonly List<Currency>? currencies;
        private HashSet<string> availableCurrencies = new HashSet<string>();
        public WalletController(ExchangeRateGenerator exchangeRateGenerator, IUserService userService)
        {
            _exchangeRateGenerator = exchangeRateGenerator;
            _userService = userService;
            currencies = _exchangeRateGenerator.GetCurrencies();
            foreach (var currency in currencies)
            {
                if (availableCurrencies.Contains(currency.Id))
                    throw new FileLoadException("Wrong currency file");

                availableCurrencies.Add(currency.Id);
            }
        }

        [HttpGet("balance")]
        [Authorize(Roles = "User")]
        public IActionResult GetCurrentUserBalances()
        {
            User? user = _userService.GetUserById(int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value));
            Dictionary<string, BalanceDTO>? result = GenerateUserBalance(user);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetUserBalanceById(int id)
        {
            User? user = _userService.GetUserById(id);
            if (user is null)
                return NotFound();
            Dictionary<string, BalanceDTO> result = GenerateUserBalance(user);
            return Ok(result);
        }

        [HttpPut("deposit/{currency}")]
        [Authorize(Roles = "User")]
        public IActionResult Deposit(string currency, [FromBody] DepositDTO depositDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (!availableCurrencies.Contains(currency))
                return BadRequest();

            Currency curr = currencies.First(c => Equals(c.Id, currency));
            if(Equals(curr.Type, "Fiat"))
            {
                if (depositDTO.Cardholder_name is null || depositDTO.Card_number is null)
                    return BadRequest();
            }
            else if (Equals(curr.Type, "Crypto"))
            {
                if (depositDTO.Address is null)
                    return BadRequest();
            }
            else
                return BadRequest();

            _userService.AddBalance(int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value), currency, depositDTO.Amount);
            return Ok();

        }

        private Dictionary<string, BalanceDTO> GenerateUserBalance(User? user)
        {
            var rates = _exchangeRateGenerator.GetCurrentRates();
            Dictionary<string, BalanceDTO>? result = new Dictionary<string, BalanceDTO>();
            foreach (string key in user.Balances.Keys)
            {
                result.Add(key, new BalanceDTO { Balance = user.Balances[key], Usd_Amount = rates[key] * user.Balances[key] });
            }

            return result;
        }
    }
}

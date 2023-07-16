using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserWallet.DTOs;
using UserWallet.Services;

namespace UserWallet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RatesController : ControllerBase
    {
        private readonly ExchangeRateGenerator _exchangeRateGenerator;
        public RatesController(ExchangeRateGenerator exchangeRateGenerator)
        {
            _exchangeRateGenerator = exchangeRateGenerator;
        }

        [HttpGet]
        public CurrentRateDTO GetCurrentRates()
        {
            CurrentRateDTO currentRate = new CurrentRateDTO();
            currentRate.DateTime = DateTime.Now;
            currentRate.Rates = _exchangeRateGenerator.GetCurrentRates();
            return currentRate;
        }
    }
}

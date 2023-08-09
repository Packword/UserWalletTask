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
            => new()
            {
                DateTime = DateTime.Now,
                Rates = _exchangeRateGenerator.GetCurrentRates()
            };
    }
}

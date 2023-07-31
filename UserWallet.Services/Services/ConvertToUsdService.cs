namespace UserWallet.Services
{
    public class ConvertToUsdService: IConvertToUsdService
    {
        private readonly ExchangeRateGenerator _exchangeRateGenerator;

        public ConvertToUsdService(ExchangeRateGenerator exchangeRateGenerator)
        {
            _exchangeRateGenerator = exchangeRateGenerator;
        }

        public decimal ConvertCurrency(string currencyId, decimal amount)
        {
            var rates = _exchangeRateGenerator.GetCurrentRates();
            return rates[currencyId] * amount;
        }
    }
}

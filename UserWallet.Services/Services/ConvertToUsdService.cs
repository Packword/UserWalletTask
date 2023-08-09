namespace UserWallet.Services
{
    public class ConvertToUsdService: IConvertToUsdService
    {
        private readonly ExchangeRateGenerator _exchangeRateGenerator;

        public ConvertToUsdService(ExchangeRateGenerator exchangeRateGenerator)
        {
            _exchangeRateGenerator = exchangeRateGenerator;
        }

        public List<(string CurrencyId, decimal UsdAmount)> ConvertCurrency(IEnumerable<(string CurrencyId, decimal Amount)> currenciesAmounts)
        {
            var rates = _exchangeRateGenerator.GetCurrentRates();
            return currenciesAmounts.Select(x => (x.CurrencyId, x.Amount *= rates[x.CurrencyId])).ToList();
        }
    }
}

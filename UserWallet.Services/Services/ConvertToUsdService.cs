namespace UserWallet.Services
{
    public class ConvertToUsdService: IConvertToUsdService
    {
        private readonly ExchangeRateGenerator _exchangeRateGenerator;

        public ConvertToUsdService(ExchangeRateGenerator exchangeRateGenerator)
        {
            _exchangeRateGenerator = exchangeRateGenerator;
        }

        public List<(string CurrencyId, decimal UsdAmount)> ConvertCurrency(IEnumerable<(string CurrencyId, decimal Amount)> balances)
        {
            var rates = _exchangeRateGenerator.GetCurrentRates();
            return balances.Select(balance => (balance.CurrencyId, balance.Amount *= rates[balance.CurrencyId])).ToList();
        }
    }
}

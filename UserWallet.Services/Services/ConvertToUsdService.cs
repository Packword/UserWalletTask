namespace UserWallet.Services
{
    public class ConvertToUsdService: IConvertToUsdService
    {
        private readonly ExchangeRateGenerator _exchangeRateGenerator;

        public ConvertToUsdService(ExchangeRateGenerator exchangeRateGenerator)
        {
            _exchangeRateGenerator = exchangeRateGenerator;
        }

        public Dictionary<string, BalanceDTO> ConvertCurrency(Dictionary<string, decimal> balances)
        {
            var rates = _exchangeRateGenerator.GetCurrentRates();
            return balances.ToDictionary(
                    x => x.Key,
                    x => new BalanceDTO
                    {
                        Amount = x.Value,
                        UsdAmount = x.Value * rates[x.Key],
                    });
        }
    }
}

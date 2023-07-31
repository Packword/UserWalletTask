namespace UserWallet.Services
{
    public class ConvertToUsdService: IConvertToUsdService
    {
        private readonly ExchangeRateGenerator _exchangeRateGenerator;

        public ConvertToUsdService(ExchangeRateGenerator exchangeRateGenerator)
        {
            _exchangeRateGenerator = exchangeRateGenerator;
        }

        public Dictionary<string, BalanceDTO> GenerateUserBalance(List<UserBalance> balances)
        {
            var rates = _exchangeRateGenerator.GetCurrentRates();

            var result = balances?.ToDictionary(x => x.CurrencyId, x => new BalanceDTO { Balance = x.Amount, UsdAmount = rates[x.CurrencyId] * x.Amount });

            return result;
        }
    }
}

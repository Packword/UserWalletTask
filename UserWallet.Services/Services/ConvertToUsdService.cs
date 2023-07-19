namespace UserWallet.Services
{
    public class ConvertToUsdService: IConvertToUsdService
    {
        private readonly ExchangeRateGenerator _exchangeRateGenerator;

        public ConvertToUsdService(ExchangeRateGenerator exchangeRateGenerator)
        {
            _exchangeRateGenerator = exchangeRateGenerator;
        }

        public Dictionary<string, BalanceDTO> GenerateUserBalance(User? user)
        {
            var rates = _exchangeRateGenerator.GetCurrentRates();
            Dictionary<string, BalanceDTO>? result = new Dictionary<string, BalanceDTO>();
            foreach (UserBalance balance in user.Balances)
            {
                result.Add(balance.CurrencyId, new BalanceDTO { Balance = balance.Amount, Usd_Amount = rates[balance.CurrencyId] * balance.Amount });
            }

            return result;
        }
    }
}

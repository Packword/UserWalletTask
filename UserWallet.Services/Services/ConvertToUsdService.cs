namespace UserWallet.Services
{
    public class ConvertToUsdService: IConvertToUsdService
    {
        private readonly ExchangeRateGenerator _exchangeRateGenerator;
        private readonly IUserBalanceService _userBalanceService;

        public ConvertToUsdService(ExchangeRateGenerator exchangeRateGenerator, IUserBalanceService userBalanceService)
        {
            _exchangeRateGenerator = exchangeRateGenerator;
            _userBalanceService = userBalanceService;
        }

        public Dictionary<string, BalanceDTO>? GenerateUserBalance(int userId)
        {
            var balances = _userBalanceService.GetUserBalances(userId);
            var rates = _exchangeRateGenerator.GetCurrentRates();

            var result = balances?.ToDictionary(x => x.CurrencyId, x => new BalanceDTO { Balance = x.Amount, UsdAmount = rates[x.CurrencyId] * x.Amount });

            return result;
        }
    }
}

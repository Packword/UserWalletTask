using UserWallet.Interfaces;

namespace UserWallet.Services
{
    public class UserBalanceService: IUserBalanceService
    {
        private readonly ApplicationDbContext _db;
        private readonly IConvertToUsdService _convertToUsdService;

        public UserBalanceService(ApplicationDbContext db, IConvertToUsdService convertToUsdService)
        {
            _db = db;
            _convertToUsdService = convertToUsdService;
        }

        public (bool Result, List<UserBalance>? Balances) GetUserBalances(int userId)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == userId);
            if (user is null)
                return (false, null);

            return (true, _db.UserBalances.Where(b => b.UserId == userId).ToList());
        }

        public Dictionary<string, BalanceDTO> ConvertToBalanceDTO(List<UserBalance>? balances)
        {
            if (balances is null)
                return new();

            var usdBalances = _convertToUsdService.ConvertCurrency(balances.Select(x => (x.CurrencyId, x.Amount)).ToList());
            var balancesZip = usdBalances.Zip(balances);

            return balancesZip.ToDictionary(
                    key => key.First.CurrencyId,
                    value => new BalanceDTO
                    {
                        Amount = value.Second.Amount,
                        UsdAmount = value.First.UsdAmount
                    }
                );
        }

        public void AddUserBalance(int userId, string currency, decimal amount)
        {
            UserBalance? balance = _db.UserBalances.FirstOrDefault(b => currency == b.CurrencyId && b.UserId == userId);

            if (balance is not null)
                balance.Amount += amount;
            else
                _db.UserBalances.Add(new()
                {
                    UserId = userId,
                    CurrencyId = currency,
                    Amount = amount
                });

            _db.SaveChanges();
        }
    }
}

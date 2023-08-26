namespace UserWallet.Services
{
    public class UserBalanceService: IUserBalanceService
    {
        private readonly ApplicationDbContext _db;

        public UserBalanceService(ApplicationDbContext db)
        {
            _db = db;
        }

        public (bool Result, List<UserBalance>? Balances) GetUserBalances(int userId)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == userId);
            if (user is null)
                return (false, null);
            else
                return (true, _db.UserBalances.Where(b => b.UserId == userId).ToList());
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

namespace UserWallet.Services
{
    public class UserBalanceService: IUserBalanceService
    {
        private readonly ApplicationDbContext _db;

        public UserBalanceService(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<UserBalance>? GetUserBalances(int userId)
        {
            return _db.Users.Include(u => u.Balances).FirstOrDefault(u => u.Id == userId)?.Balances?.ToList();
        }

        public void AddBalance(int userId, string currency, decimal amount)
        {
            UserBalance? balance = _db.UserBalances.FirstOrDefault(b => Equals(currency, b.CurrencyId) && Equals(b.UserId, userId));

            if (balance is not null)
                balance.Amount += amount;
            else
            {
                _db.UserBalances.Add(new UserBalance
                {
                    UserId = userId,
                    CurrencyId = currency,
                    Amount = amount
                });
            }
            _db.SaveChanges();
        }
    }
}

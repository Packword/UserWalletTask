namespace UserWallet.Services
{
    public class DepositCryptoService: IDepositCryptoService
    {
        private readonly ApplicationDbContext _db;
        public DepositCryptoService(ApplicationDbContext db)
        {
            _db = db;
        }
        public bool CreateDeposit(DepositDTO deposit, int userId, string currencyId)
        {
            if (deposit.Address is null)
            {
                return false;
            }
            else
            {
                Deposit newDeposit = new Deposit
                {
                    UserId = userId,
                    CurrencyId = currencyId,
                    Amount = deposit.Amount,
                    Status = "Undecided",
                    Address = deposit.Address
                };
                _db.Deposits.Add(newDeposit);
                _db.SaveChanges();
                return true;
            }
        }
    }
}

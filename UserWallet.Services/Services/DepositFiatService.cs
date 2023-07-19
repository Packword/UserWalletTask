using Microsoft.EntityFrameworkCore;

namespace UserWallet.Services
{
    public class DepositFiatService : IDepositFiatService
    {
        private readonly ApplicationDbContext _db;
        public DepositFiatService(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool CreateDeposit(DepositDTO deposit, int userId, string currencyId)
        {
            if(deposit.Card_number is null || deposit.Cardholder_name is null)
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
                    CardholderName = deposit.Cardholder_name,
                    CardNumber = deposit.Card_number
                };
                _db.Deposits.Add(newDeposit);
                _db.SaveChanges();
                return true;
            }
        }
    }
}

namespace UserWallet.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserService _userService;

        public TransactionService(ApplicationDbContext db, IUserService userService)
        {
            _db = db;
            _userService = userService;
        }

        public List<Deposit>? GetAllDeposits()
        {
            return _db.Deposits.ToList();
        }
        public List<Deposit>? GetUserDeposits(int userId)
        {
            return _db.Deposits.Where(d => d.UserId == userId).ToList();
        }

        public bool ApproveTransaction(int txId)
        {
            var transaction = _db.Deposits.FirstOrDefault(d => d.Id == txId);
            if(transaction is null)
                return false;

            transaction.Status = "Approved";
            _userService.AddBalance(transaction.UserId, transaction.CurrencyId, transaction.Amount);
            return true;
        }

        public bool DeclineTransaction(int txId)
        {
            var transaction = _db.Deposits.FirstOrDefault(d => d.Id == txId);
            if (transaction is null)
                return false;

            transaction.Status = "Declined";
            return true;
        }
    }
}

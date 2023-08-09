namespace UserWallet.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserBalanceService _userBalanceService;

        public TransactionService(ApplicationDbContext db, IUserBalanceService userBalanceService)
        {
            _db = db;
            _userBalanceService = userBalanceService;
        }

        public List<Deposit> GetAllDeposits()
            => _db.Deposits.ToList();

        public List<Deposit> GetUserDeposits(int userId)
            => _db.Deposits.Where(d => d.UserId == userId).ToList();

        public bool ApproveTransaction(int txId)
        {
            var transaction = _db.Deposits.FirstOrDefault(d => d.Id == txId);
            if(transaction is null || transaction.Status != DepositStatus.Undecided)
                return false;

            transaction.Status = DepositStatus.Approved;
            _userBalanceService.AddUserBalance(transaction.UserId, transaction.CurrencyId, transaction.Amount);
            _db.SaveChanges();
            return true;
        }

        public bool DeclineTransaction(int txId)
        {
            var transaction = GetTransactionById(txId);
            if (transaction is null || transaction.Status != DepositStatus.Undecided)
                return false;

            transaction.Status = DepositStatus.Declined;
            _db.SaveChanges();
            return true;
        }

        public Deposit? GetTransactionById(int txId)
            => _db.Deposits.FirstOrDefault(d => d.Id == txId);
    }
}

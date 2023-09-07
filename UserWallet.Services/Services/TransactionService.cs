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

        public List<Deposit> GetDepositsWithFiltration(IEnumerable<int>? usersId = null,
                                                       IEnumerable<string>? currenciesId = null,
                                                       IEnumerable<DepositStatus>? statuses = null,
                                                       bool byDate = false)
        {
            var transactions = _db.Deposits.AsQueryable();
            if (currenciesId is not null)
                transactions = transactions.Where(t => currenciesId.Contains(t.CurrencyId));
            if (statuses is not null)
                transactions = transactions.Where(s => statuses.Contains(s.Status));
            if (usersId is not null)
                transactions = transactions.Where(t => usersId.Contains(t.UserId));
            if (byDate)
                transactions = transactions.OrderByDescending(t => t.CreatedOn);

            return transactions.ToList();
        }

        public List<Deposit> GetUserDeposits(int userId)
            => _db.Deposits.Where(d => d.UserId == userId).ToList();

        public (ServiceResult Result, string Message) DecideTransactionStatus(int txId, DepositStatus status)
        {
            var transaction = _db.Deposits.FirstOrDefault(d => d.Id == txId);
            if(transaction is null)
                return (ServiceResult.NotFound, "Transaction not found");
            if (transaction.Status != DepositStatus.Undecided)
                return (ServiceResult.NotValid, "Deposit status already decided");

            transaction.Status = status;
            if (status is DepositStatus.Approved)
            {
                _userBalanceService.AddUserBalance(transaction.UserId, transaction.CurrencyId, transaction.Amount);
            }
            _db.SaveChanges();
            return (ServiceResult.Success, "");
        }

        public Deposit? GetTransactionById(int txId)
            => _db.Deposits.FirstOrDefault(d => d.Id == txId);
    }
}

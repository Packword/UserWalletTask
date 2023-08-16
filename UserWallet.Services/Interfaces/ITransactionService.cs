namespace UserWallet.Interfaces
{
    public interface ITransactionService
    {
        public List<Deposit> GetUserDeposits(int userId);
        public List<Deposit> GetAllDeposits();
        public (ServiceResult Result, string Message) ApproveTransaction(int txId);
        public (ServiceResult Result, string Message) DeclineTransaction(int txId);
        public Deposit? GetTransactionById(int txId);
    }
}

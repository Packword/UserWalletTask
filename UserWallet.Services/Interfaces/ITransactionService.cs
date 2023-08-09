namespace UserWallet.Interfaces
{
    public interface ITransactionService
    {
        public List<Deposit> GetUserDeposits(int userId);
        public List<Deposit> GetAllDeposits();
        public bool ApproveTransaction(int txId);
        public bool DeclineTransaction(int txId);
    }
}

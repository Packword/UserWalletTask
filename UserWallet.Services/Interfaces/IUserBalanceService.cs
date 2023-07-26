namespace UserWallet.Interfaces
{
    public interface IUserBalanceService
    {
        public List<UserBalance>? GetUserBalances(int userId);
        public void AddBalance(int userId, string currency, decimal amount);
    }
}

namespace UserWallet.Interfaces
{
    public interface IUserBalanceService
    {
        public List<UserBalance>? GetUserBalances(int userId);
        public void AddUserBalance(int userId, string currency, decimal amount);
    }
}

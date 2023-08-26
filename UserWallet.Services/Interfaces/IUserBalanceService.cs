namespace UserWallet.Interfaces
{
    public interface IUserBalanceService
    {
        public (bool Result, List<UserBalance>? Balances) GetUserBalances(int userId);
        public void AddUserBalance(int userId, string currency, decimal amount);
    }
}

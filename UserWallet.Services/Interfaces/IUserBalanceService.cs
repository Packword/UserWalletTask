namespace UserWallet.Interfaces
{
    public interface IUserBalanceService
    {
        public Dictionary<string, BalanceDTO> ConvertToBalanceDTO(List<UserBalance>? balances);
        public (bool Result, List<UserBalance>? Balances) GetUserBalances(int userId);
        public void AddUserBalance(int userId, string currency, decimal amount);
    }
}

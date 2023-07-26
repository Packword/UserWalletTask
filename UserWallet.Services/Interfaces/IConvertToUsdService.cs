namespace UserWallet.Interfaces
{
    public interface IConvertToUsdService
    {
        public Dictionary<string, BalanceDTO>? GenerateUserBalance(int userId);
    }
}

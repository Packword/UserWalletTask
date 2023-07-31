namespace UserWallet.Interfaces
{
    public interface IConvertToUsdService
    {
        public Dictionary<string, BalanceDTO>? GenerateUserBalance(List<UserBalance> userId);
    }
}

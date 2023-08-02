namespace UserWallet.Interfaces
{
    public interface IConvertToUsdService
    {
        public Dictionary<string, BalanceDTO> ConvertCurrency(Dictionary<string, decimal> balances);
    }
}

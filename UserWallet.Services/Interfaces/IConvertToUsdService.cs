namespace UserWallet.Interfaces
{
    public interface IConvertToUsdService
    {
        public List<(string CurrencyId, decimal UsdAmount)> ConvertCurrency(IEnumerable<(string CurrencyId, decimal Amount)> balances);
    }
}

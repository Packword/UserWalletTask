namespace UserWallet.Interfaces
{
    public interface IConvertToUsdService
    {
        public decimal ConvertCurrency(string currencyId, decimal amount);
    }
}

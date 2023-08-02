namespace UserWallet.Interfaces
{
    public interface IDepositCryptoService
    {
        public bool CreateDeposit(int userId, DepositDTO deposit, string currencyId);
    }
}

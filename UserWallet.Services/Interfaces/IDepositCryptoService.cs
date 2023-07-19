namespace UserWallet.Interfaces
{
    public interface IDepositCryptoService
    {
        public bool CreateDeposit(DepositDTO deposit, int userId, string currencyId);
    }
}

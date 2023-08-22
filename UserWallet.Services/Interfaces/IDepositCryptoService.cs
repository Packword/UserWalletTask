namespace UserWallet.Interfaces
{
    public interface IDepositCryptoService
    {
        public (bool Result, string Message) CreateDeposit(int userId, DepositDTO deposit, string currencyId);
    }
}

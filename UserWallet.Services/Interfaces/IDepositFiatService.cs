namespace UserWallet.Interfaces
{
    public interface IDepositFiatService
    {
        public bool CreateDeposit(int userId, DepositDTO deposit, string currencyId);
    }
}

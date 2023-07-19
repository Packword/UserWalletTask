namespace UserWallet.Interfaces
{
    public interface IDepositFiatService
    {
        public bool CreateDeposit(DepositDTO deposit, int userId, string currencyId);
    }
}

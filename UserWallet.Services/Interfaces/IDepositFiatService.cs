namespace UserWallet.Interfaces
{
    public interface IDepositFiatService
    {
        public (bool Result, string Message) CreateDeposit(int userId, DepositDTO deposit, string currencyId);
    }
}

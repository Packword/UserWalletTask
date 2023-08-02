namespace UserWallet.Services
{
    public class DepositFiatService : IDepositFiatService
    {
        private const int CARDNUMBER_LENGTH = 16;
        private const int CARDHOLDER_MINLENGTH = 2;
        private const int CARDHOLDER_MAXLENGTH = 16;

        private readonly ApplicationDbContext _db;
        public DepositFiatService(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool CreateDeposit(int userId, DepositDTO deposit, string currencyId)
        {
            if (!IsAdditionalDataValid(deposit))
                return false;

            Deposit newDeposit = new Deposit
            {
                UserId = userId,
                CurrencyId = currencyId,
                Amount = deposit.Amount,
                Status = DepositStatus.Undecided,
                AdditionalData = JsonSerializer.Serialize(new FiatDepositAdditionalDataDTO
                {
                    CardholderName = deposit.CardholderName!,
                    CardNumber = deposit.CardNumber!
                })
            };
            _db.Deposits.Add(newDeposit);
            _db.SaveChanges();
            return true;
        }

        private static bool IsAdditionalDataValid(DepositDTO deposit)
             => deposit.CardNumber?.Length >= CARDNUMBER_LENGTH
                && deposit.CardholderName?.Length is >= CARDHOLDER_MINLENGTH and <= CARDHOLDER_MAXLENGTH;
    }
}

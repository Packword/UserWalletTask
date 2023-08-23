namespace UserWallet.Services
{
    public class DepositFiatService : IDepositFiatService
    {
        private const int CARDNUMBER_LENGTH = 16;
        private const int CARDHOLDER_MINLENGTH = 2;
        private const int CARDHOLDER_MAXLENGTH = 16; 
        private const decimal AMOUNT_MIN_VALUE = 0.1m;
        private const decimal AMOUNT_MAX_VALUE = 100m;

        private readonly ApplicationDbContext _db;
        public DepositFiatService(ApplicationDbContext db)
        {
            _db = db;
        }

        public (bool Result, string Message) CreateDeposit(int userId, DepositDTO deposit, string currencyId)
        {
            var validationResult = ValidateAdditionalData(deposit);
            if (!validationResult.Result)
                return validationResult;

            Deposit newDeposit = new()
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
            return validationResult;
        }

        private static (bool Result, string Message) ValidateAdditionalData(DepositDTO deposit)
        {
            if (deposit.CardNumber?.Length != CARDNUMBER_LENGTH)
                return (false, $"Cardnumber must contain {CARDNUMBER_LENGTH} characters");
            else if (deposit.CardholderName?.Length is < CARDHOLDER_MINLENGTH)
                return (false, $"Cardholder name must contain atleast {CARDHOLDER_MINLENGTH} characters");
            else if (deposit.CardholderName?.Length is > CARDHOLDER_MAXLENGTH)
                return (false, $"Cardholder name must be less than {CARDHOLDER_MAXLENGTH} characters");
            else if (deposit.Amount is < AMOUNT_MIN_VALUE)
                return (false, $"Amount must be atleast {AMOUNT_MIN_VALUE}");
            else if (deposit.Amount is > AMOUNT_MAX_VALUE)
                return (false, $"Amount must be less than {AMOUNT_MAX_VALUE}");
            else
                return (true, "");
        }
    }
}

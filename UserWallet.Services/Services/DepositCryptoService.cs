namespace UserWallet.Services
{
    public class DepositCryptoService : IDepositCryptoService
    {
        private const int ADDRESS_LENGTH = 16;
        private const decimal AMOUNT_MIN_VALUE = 0.1m;
        private const decimal AMOUNT_MAX_VALUE = 100m;

        private readonly ApplicationDbContext _db;
        public DepositCryptoService(ApplicationDbContext db)
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
                AdditionalData = JsonSerializer.Serialize(new CryptoDepositAdditionalDataDTO
                {
                    Address = deposit.Address!
                })
            };
            _db.Deposits.Add(newDeposit);
            _db.SaveChanges();
            return validationResult;
        }

        private static (bool Result, string Message) ValidateAdditionalData(DepositDTO deposit) {
            if (deposit.Address?.Length != ADDRESS_LENGTH)
                return (false, $"Address must contain {ADDRESS_LENGTH} characters");
            else if (deposit.Amount is <= AMOUNT_MIN_VALUE)
                return (false, $"Amount must be atleast {AMOUNT_MIN_VALUE}");
            else if (deposit.Amount is <= AMOUNT_MAX_VALUE)
                return (false, $"Amount must be less than {AMOUNT_MAX_VALUE}");
            else
                return (true, "");
        }
}
}

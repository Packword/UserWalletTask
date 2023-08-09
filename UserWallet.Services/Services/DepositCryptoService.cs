namespace UserWallet.Services
{
    public class DepositCryptoService : IDepositCryptoService
    {
        private const int ADDRESS_LENGTH = 16;

        private readonly ApplicationDbContext _db;
        public DepositCryptoService(ApplicationDbContext db)
        {
            _db = db;
        }
        public bool CreateDeposit(int userId, DepositDTO deposit, string currencyId)
        {
            if (!IsAdditionalDataValid(deposit))
                return false;

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
            return true;
        }

        private static bool IsAdditionalDataValid(DepositDTO deposit)
            => deposit.Address?.Length == ADDRESS_LENGTH;
    }
}

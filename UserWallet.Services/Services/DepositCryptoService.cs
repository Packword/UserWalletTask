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
        public bool CreateDeposit(DepositDTO deposit, int userId, string currencyId)
        {
            if (!IsCryptoAdditionalDataValid(deposit))
                return false;


            Deposit newDeposit = new Deposit
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

        private static bool IsCryptoAdditionalDataValid(DepositDTO deposit)
            => deposit.Address?.Length == ADDRESS_LENGTH;
    }
}

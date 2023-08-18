namespace UserWallet.Services
{
    public class CurrencyService: ICurrencyService
    {
        private readonly ApplicationDbContext _db;
        public CurrencyService(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<Currency> GetCurrencies()
            => _db.Currencies.ToList();
    }
}

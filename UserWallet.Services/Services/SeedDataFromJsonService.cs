
namespace UserWallet.Services
{
    public class SeedDataFromJsonService : BackgroundService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private const string CURRENCIES_JSON_PATH = "./../UserWallet.Data/Data/Currencies.json";
        private const string USERS_JSON_PATH = "./../UserWallet.Data/Data/Users.json";
        public SeedDataFromJsonService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var context = _contextFactory.CreateDbContext();
            using (var fs = new FileStream(CURRENCIES_JSON_PATH, FileMode.Open))
                MigrateCurrenciesFromJsonToDb(context, fs);

            if (!context.Users.Any())
            {
                using var fs = new FileStream(USERS_JSON_PATH, FileMode.Open);
                MigrateUsersFromJsonToDb(context, fs);
            }

            context.SaveChanges();
            return Task.CompletedTask;
        }

        private static void MigrateUsersFromJsonToDb(ApplicationDbContext context, FileStream fs)
        {
            var tmpUsers = JsonSerializer.Deserialize<List<TmpUser>>(fs)!;
            foreach (var tmpUser in tmpUsers)
            {
                AddUserToDb(context, tmpUser);
            }
        }

        private static void AddUserToDb(ApplicationDbContext context, TmpUser tmpUser)
        {
            User user = new(tmpUser.Username, tmpUser.Password, tmpUser.Role, false);
            FillUserBalances(user, tmpUser);
            context.Users.Add(user);
        }

        private static void FillUserBalances(User user, TmpUser tmpUser)
            => user.Balances = tmpUser.Balances
                                   .Select(balance => new UserBalance { CurrencyId = balance.Key, Amount = balance.Value })
                                   .ToList();

        private static void MigrateCurrenciesFromJsonToDb(ApplicationDbContext context, FileStream fs)
        {
            var tmpCurrencies = JsonSerializer.Deserialize<List<TmpCurrency>>(fs)!;
            var oldCurrencies = context.Currencies.ToDictionary(c => c.Id);

            foreach (var currId in oldCurrencies.Keys)
            {
                oldCurrencies[currId].IsAvailable = false;
            }

            foreach (var tmpCurr in tmpCurrencies)
            {
                oldCurrencies.TryGetValue(tmpCurr.Id, out var sameCurrency);

                if (sameCurrency is null)
                    AddNewCurrencyToDb(context, tmpCurr);
                else
                    sameCurrency.IsAvailable = true;
            }
        }

        private static void AddNewCurrencyToDb(ApplicationDbContext context, TmpCurrency tmpCurr)
            => context.Currencies.Add(new()
            {
                Id = tmpCurr.Id,
                Type = Enum.Parse<CurrencyType>(tmpCurr.Type),
                IsAvailable = true
            });

        private class TmpUser
        {
            public string Username { get; set; } = null!;
            public string Password { get; set; } = null!;
            public string Role { get; set; } = null!;
            public Dictionary<string, decimal> Balances { get; set; } = new();
        }

        private class TmpCurrency
        {
            public string Id { get; set; } = null!;
            public string Type { get; set; } = null!;
        }
    }
}

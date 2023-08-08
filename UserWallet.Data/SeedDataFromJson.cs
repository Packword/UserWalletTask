namespace UserWallet.Data
{
    public static class SeedDataFromJson
    {
        private const string CURRENCIES_JSON_PATH = "./../UserWallet.Data/Data/Currencies.json";
        private const string USERS_JSON_PATH = "./../UserWallet.Data/Data/Users.json";

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
            
            using (var fs = new FileStream(CURRENCIES_JSON_PATH, FileMode.Open))
                MigrateCurrenciesFromJsonToDb(context, fs);

            if (!context.Users.Any())
            {
                using var fs = new FileStream(USERS_JSON_PATH, FileMode.Open);
                MigrateUsersFromJsonToDb(context, fs);
            }

            context.SaveChanges();
        }

        private static void MigrateUsersFromJsonToDb(ApplicationDbContext context, FileStream fs)
        {
            var tmpUsers = JsonSerializer.Deserialize<List<TmpUser>>(fs)!;
            foreach (var tmpUser in tmpUsers)
                AddUserToDb(context, tmpUser);
        }

        private static void AddUserToDb(ApplicationDbContext context, TmpUser tmpUser)
        {
            User user = new User
            {
                Username = tmpUser.Username,
                Password = tmpUser.Password,
                Role = tmpUser.Role
            };
            FillUserBalances(user, tmpUser);
            context.Users.Add(user);
        }

        private static void FillUserBalances(User user, TmpUser tmpUser)
        {
            user.Balances = tmpUser.Balances
                                   .Select(balance => new UserBalance { CurrencyId = balance.Key, Amount = balance.Value })
                                   .ToList();
        }

        private static void MigrateCurrenciesFromJsonToDb(ApplicationDbContext context, FileStream fs)
        {
            var tmpCurrencies = JsonSerializer.Deserialize<List<TmpCurrency>>(fs)!;
            var oldCurrencies = context.Currencies.ToDictionary(c => c.Id);

            foreach (var currId in oldCurrencies.Keys)
                oldCurrencies[currId].IsAvailable = false;

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
        {
            context.Currencies.Add(new Currency
            {
                Id = tmpCurr.Id,
                Type = Enum.Parse<CurrencyType>(tmpCurr.Type),
                IsAvailable = true
            });
        }

        private class TmpUser
        {
            public string Username { get; set; } = null!;
            public string Password { get; set; } = null!;
            public string Role { get; set; } = null!;
            public Dictionary<string, decimal> Balances { get; set; } = new Dictionary<string, decimal>();
        }

        private class TmpCurrency
        {
            public string Id { get; set; } = null!;
            public string Type { get; set; } = null!;
        }
    }
}

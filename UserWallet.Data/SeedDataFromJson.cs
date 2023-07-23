using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using UserWallet.Models;

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

            if (context.Users.Any())
                context.SaveChanges();
            else
            {
                using var fs = new FileStream(USERS_JSON_PATH, FileMode.Open);
                MigrateUsersFromJsonToDb(context, fs);
                context.SaveChanges();
            }
        }

        private static void MigrateUsersFromJsonToDb(ApplicationDbContext context, FileStream fs)
        {
            List<TmpUser> tmpUsers = JsonSerializer.Deserialize<List<TmpUser>>(fs);
            foreach (TmpUser tmpUser in tmpUsers)
                ParseJsonUserAndAddToDb(context, tmpUser);
        }

        private static void ParseJsonUserAndAddToDb(ApplicationDbContext context, TmpUser tmpUser)
        {
            User user = new User
            {
                Username = tmpUser.Username,
                Password = tmpUser.Password,
                Role = tmpUser.Role,
                IsBlocked = false
            };
            ParseUserBalancesAndAddToDb(context, tmpUser, user);
            context.Users.Add(user);
        }

        private static void ParseUserBalancesAndAddToDb(ApplicationDbContext context, TmpUser tmpUser, User user)
        {
            user.Balances = new List<UserBalance>();
            foreach (var key in tmpUser.Balances.Keys)
            {
                UserBalance userBalance = new UserBalance
                {
                    CurrencyId = key,
                    Amount = tmpUser.Balances[key]
                };
                user.Balances.Add(userBalance);
            }
        }

        private static void MigrateCurrenciesFromJsonToDb(ApplicationDbContext context, FileStream fs)
        {
            List<TmpCurrency>? tmpCurrencies = JsonSerializer.Deserialize<List<TmpCurrency>>(fs);
            List<Currency> oldCurrencies = context.Currencies.ToList();
            foreach (var oldCurr in oldCurrencies)
                oldCurr.IsAvailable = false;

            foreach (var tmpCurr in tmpCurrencies)
            {
                var sameCurrency = oldCurrencies.FirstOrDefault(c => c.Id == tmpCurr.Id);

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
                Type = tmpCurr.Type,
                IsAvailable = true
            });
        }

        private class TmpUser
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }
            public Dictionary<string, decimal> Balances { get; set; } = new Dictionary<string, decimal>();
        }

        private class TmpCurrency
        {
            public string Id { get; set; }
            public string Type { get; set; }
        }
    }
}

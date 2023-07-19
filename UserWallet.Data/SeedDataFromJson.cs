using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using UserWallet.Models;

namespace UserWallet.Data
{
    public static class SeedDataFromJson
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
            
            using (var fs = new FileStream("./../UserWallet.Data/Data/Currencies.json", FileMode.Open))
            {
                List<TmpCurrency>? tmpCurrencies = JsonSerializer.Deserialize<List<TmpCurrency>>(fs);
                List<Currency> oldCurrencies = context.Currencies.ToList();
                foreach (var oldCurr in oldCurrencies)
                    oldCurr.IsAvailable = false;

                foreach(var tmpCurr in tmpCurrencies)
                {
                    var sameCurrency = oldCurrencies.FirstOrDefault(c => c.Id == tmpCurr.Id);
                    if (sameCurrency is null)
                    {
                        context.Currencies.Add(new Currency
                        {
                            Id = tmpCurr.Id,
                            Type = tmpCurr.Type,
                            IsAvailable = true
                        });
                    }
                    else
                        sameCurrency.IsAvailable = true;
                }
            }
            if (context.Users.Any())
            {
                context.SaveChanges();
                return;
            }
            using (var fs = new FileStream("./../UserWallet.Data/Data/Users.json", FileMode.Open))
            {
                List<TmpUser> tmpUsers = JsonSerializer.Deserialize<List<TmpUser>>(fs);
                foreach (TmpUser tmpUser in tmpUsers)
                {
                    User user = new User
                    {
                        Username = tmpUser.Username,
                        Password = tmpUser.Password,
                        Role = tmpUser.Role,
                        IsBlocked = false
                    };
                    user.Balances = new List<UserBalance>();
                    foreach (var key in tmpUser.Balances.Keys)
                    {
                        UserBalance userBalance = new UserBalance
                        {
                            UserId = user.Id,
                            CurrencyId = key,
                            Amount = tmpUser.Balances[key]
                        };
                        user.Balances.Add(userBalance);
                        context.UserBalances.Add(userBalance);
                    }
                    context.Users.Add(user);
                }
            }
            context.SaveChanges();
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

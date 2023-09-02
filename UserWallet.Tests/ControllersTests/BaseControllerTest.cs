using Microsoft.Extensions.Options;
using NUnit.Framework.Internal;
using System.Configuration;
using UserWallet.OptionsModels;

namespace UserWallet.Tests.ControllersTests
{
    public abstract class BaseControllerTest
    {
        protected const string  DEFAULT_USER_USERNAME = "maxim";
        protected const string  DEFAULT_USER_PASSWORD = "123456";
        protected const string  DEFAULT_USER_ROLE = UsersRole.USER;
        protected const int     DEFAULT_USER_ID = 2;
        protected const string  ADMIN_USERNAME = "Admin";
        protected const string  ADMIN_PASSWORD = "1234";
        protected const string  ADMIN_ROLE = UsersRole.ADMIN;
        protected const int     ADMIN_ID = 1;
        protected const int     DEFAULT_USERS_COUNT = 2;
        protected const string  CRYPTO_CURRENCY_ID = "btc";
        protected const string  CRYPTO_ADDRESS = "1234567890123456";
        protected const string  FIAT_CURRENCY_ID = "rub";
        protected const string  FIAT_CARDHOLDER = "1234567890123456";
        protected const string  FIAT_CARDNUMBER = "1234567890123456";
        protected const decimal DEFAULT_DEPOSIT_AMOUNT = 50;
        protected readonly TimeSpan EXCHANGE_UPDATE_INTERVAL = TimeSpan.FromSeconds(1);

        protected WebApplicationFactory<Program> Factory { get; set; }
        protected HttpClient Client { get; set; }

        [OneTimeSetUp]
        public virtual void OneTimeSetup()
        {
            Factory = WebApplicationFactoryHelper.CreateFactoryWithInMemoryDb();
        }

        [SetUp]
        public void Setup()
        {
            Client = Factory.CreateClient();
            var sp = Factory.Services;
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;

            var exchangeOptions = scopedServices.GetRequiredService<IOptionsMonitor<ExchangeRateGeneratorOptions>>().CurrentValue;
            exchangeOptions.UpdateInterval = EXCHANGE_UPDATE_INTERVAL;
            var db = scopedServices.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Users.Add(
                new User(
                    ADMIN_USERNAME,
                    ADMIN_PASSWORD,
                    ADMIN_ROLE,
                    false
                ));
            db.Users.Add(
                new User(
                    DEFAULT_USER_USERNAME,
                    DEFAULT_USER_PASSWORD,
                    DEFAULT_USER_ROLE,
                    false
                ));

            db.Currencies.Add(new Currency
            {
                Id = CRYPTO_CURRENCY_ID,
                IsAvailable = true,
                Type = CurrencyType.Crypto
            });
            db.Currencies.Add(new Currency
            {
                Id = FIAT_CURRENCY_ID,
                IsAvailable = true,
                Type = CurrencyType.Fiat
            });

            db.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            Client.Dispose();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Factory.Dispose();
        }

        protected async static Task<HttpResponseMessage> LoginAsAdmin(HttpClient client)
            => await client.Login(ADMIN_USERNAME, ADMIN_PASSWORD);

        protected async static Task<HttpResponseMessage> LoginAsUser(HttpClient client)
            => await client.Login(DEFAULT_USER_USERNAME, DEFAULT_USER_PASSWORD);
    }
}

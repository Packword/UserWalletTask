using NUnit.Framework.Internal;

namespace UserWallet.Tests.ControllersTests
{
    public abstract class BaseControllerTest
    {
        private const string WorkingDirectoryPath = "../../../../UserWallet";

        private string initialDirectory;

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

        protected WebApplicationFactory<Program> _factory;
        protected HttpClient _client;

        [OneTimeSetUp]
        public virtual void OneTimeSetup()
        {
            initialDirectory = Directory.GetCurrentDirectory();

            Directory.SetCurrentDirectory(WorkingDirectoryPath);
            _factory = WebApplicationFactoryHelper.CreateFactoryWithInMemoryDb();
        }

        [SetUp]
        public void Setup()
        {
            _client = _factory.CreateClient(); ;

            var sp = _factory.Services;
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;

            var db = scopedServices.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Users.Add(
                UserServiceHelper.CreateUser(
                    ADMIN_USERNAME,
                    ADMIN_PASSWORD,
                    ADMIN_ROLE,
                    false
                ));
            db.Users.Add(
                UserServiceHelper.CreateUser(
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
            _client.Dispose();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Directory.SetCurrentDirectory(initialDirectory);
            _factory.Dispose();
        }

        protected async static Task<HttpResponseMessage> LoginAsAdmin(HttpClient client)
            => await client.Login(ADMIN_USERNAME, ADMIN_PASSWORD);

        protected async static Task<HttpResponseMessage> LoginAsUser(HttpClient client)
            => await client.Login(DEFAULT_USER_USERNAME, DEFAULT_USER_PASSWORD);
    }
}

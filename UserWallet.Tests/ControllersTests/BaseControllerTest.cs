namespace UserWallet.Tests.ControllersTests
{
    public abstract class BaseControllerTest
    {
        private const string WorkingDirectoryPath = "../../../../UserWallet";
        protected WebApplicationFactory<Program> _factory;
        protected AuthServiceHelper _authServiceHelper;
        protected WebApplicationFactoryHelper _factoryHelper = new WebApplicationFactoryHelper();
        protected HttpClient _client;
        protected string initialDirectory;

        [OneTimeSetUp]
        public virtual void OneTimeSetup()
        {
            initialDirectory = Directory.GetCurrentDirectory();

            Directory.SetCurrentDirectory(WorkingDirectoryPath);
            _factory = _factoryHelper.CreateFactoryWithInMemoryDb();
        }

        [SetUp]
        public void Setup()
        {
            _client = _factory.CreateClient();
            _authServiceHelper = new AuthServiceHelper(_client);
            var sp = _factory.Services;
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            db.Users.Add(new User
            {
                Username = TestData.ADMIN_USERNAME,
                Password = TestData.ADMIN_PASSWORD,
                Role = UsersRole.ADMIN,
                IsBlocked = false
            });
            db.Users.Add(new User
            {
                Username = TestData.DEFAULT_USER_USERNAME,
                Password = TestData.DEFAULT_USER_PASSWORD,
                Role = UsersRole.USER,
                IsBlocked = false
            }); 
            db.Currencies.Add(new Currency
            {
                Id = TestData.CRYPTO_CURRENCY_ID,
                IsAvailable = true,
                Type = CurrencyType.Crypto
            });
            db.Currencies.Add(new Currency
            {
                Id = TestData.FIAT_CURRENCY_ID,
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
    }
}

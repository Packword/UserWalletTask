namespace UserWallet.Tests.ControllersTests
{
    public class BaseControllerTest
    {
        protected WebApplicationFactory<Program> _factory;
        protected WebApplicationFactoryHelper _factoryHelper = new WebApplicationFactoryHelper();
        protected ApplicationDbContext db;
        protected HttpClient _client;
        protected string initialDirectory;

        [OneTimeSetUp]
        public virtual void OneTimeSetup()
        {
            initialDirectory = Directory.GetCurrentDirectory();

            Directory.SetCurrentDirectory("../../../../UserWallet");
            _factory = _factoryHelper.CreateFactoryWithInMemoryDb();
        }

        [SetUp]
        public async virtual Task Setup()
        {
            _client = _factory.CreateClient();
            var sp = _factory.Services;
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            db = scopedServices.GetRequiredService<ApplicationDbContext>();
            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();
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
                Id = "btc",
                IsAvailable = true,
                Type = CurrencyType.Crypto
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

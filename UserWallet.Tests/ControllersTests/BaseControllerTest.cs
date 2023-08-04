namespace UserWallet.Tests.ControllersTests
{
    public class BaseControllerTest
    {
        protected WebApplicationFactory<Program> _factory;
        protected WebApplicationFactoryHelper _factoryHelper = new WebApplicationFactoryHelper();
        protected AuthServiceHelper _authServiceHelper;
        protected HttpClient _client;
        protected string initialDirectory;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            initialDirectory = Directory.GetCurrentDirectory();

            Directory.SetCurrentDirectory("../../../../UserWallet");
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
            SeedDataFromJson.Initialize(scopedServices);
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

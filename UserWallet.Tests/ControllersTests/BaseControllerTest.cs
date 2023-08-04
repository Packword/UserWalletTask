namespace UserWallet.Tests.ControllersTests
{
    public class BaseControllerTest
    {
        protected WebApplicationFactory<Program> _factory;
        protected WebApplicationFactoryHelper _factoryHelper = new WebApplicationFactoryHelper();
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
            var db = scopedServices.GetRequiredService<ApplicationDbContext>();
            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();
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

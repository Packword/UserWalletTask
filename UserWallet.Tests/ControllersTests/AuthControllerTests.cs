namespace UserWallet.Tests.ControllersTests
{
    public class AuthTests
    {
        private WebApplicationFactory<Program> _factory;
        private WebApplicationFactoryHelper _factoryHelper = new WebApplicationFactoryHelper();
        private AuthServiceHelper _authServiceHelper;
        private HttpClient _client;
        private string initialDirectory;

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

        [Test]
        public async Task PositiveLoginAsAdminTest()
        {
            var response = await _authServiceHelper.Login("Admin", "1234");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task PositiveLoginAsUserTest()
        {
            var response = await _authServiceHelper.Login("maxim", "123456");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task PositiveChangePasswordTest()
        {
            await _authServiceHelper.Login("Admin", "1234");
            var requestMessage = new HttpRequestMessage(HttpMethod.Patch, "/auth/change-password");
            requestMessage.Content = JsonContent.Create(new ChangeUserPasswordDTO { NewPassword = "12345" });
            var response = await _client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task LoginAfterChangePasswordTest()
        {
            await _authServiceHelper.Login("Admin", "1234");
            var requestMessage = new HttpRequestMessage(HttpMethod.Patch, "/auth/change-password");
            requestMessage.Content = JsonContent.Create(new ChangeUserPasswordDTO { NewPassword = "12345" });
            var response = await _client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            await _authServiceHelper.Logout();
            response = await _authServiceHelper.Login("Admin", "12345");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

        }

        [Test]
        public async Task PositiveLogoutTest()
        {
            await _authServiceHelper.Login("Admin", "1234");
            HttpResponseMessage response = await _authServiceHelper.Logout();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task PositiveSignInTest()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/auth/sign-up");
            requestMessage.Content = JsonContent.Create(new SignUpDTO
            {
                Username = "ForTest",
                Password = "Test"
            });
            var response = await _client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response = await _authServiceHelper.Login("ForTest", "Test");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
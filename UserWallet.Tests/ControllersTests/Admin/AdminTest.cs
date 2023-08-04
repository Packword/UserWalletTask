namespace UserWallet.Tests.ControllersTests.Admin
{
    public class AdminTest: BaseControllerTest
    {
        protected AuthServiceHelper _authServiceHelper;
        [SetUp]
        public async override Task Setup()
        {
            await base.Setup();
            _authServiceHelper = new AuthServiceHelper(_client);
            await _authServiceHelper.LoginAsAdmin();
        }
    }
}

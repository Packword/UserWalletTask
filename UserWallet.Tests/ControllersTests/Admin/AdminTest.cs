namespace UserWallet.Tests.ControllersTests.Admin
{
    public class AdminTest: BaseControllerTest
    {
        [SetUp]
        public async override Task Setup()
        {
            await base.Setup();
            await _authServiceHelper.LoginAsync("Admin", "1234");
        }
    }
}

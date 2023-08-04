namespace UserWallet.Tests.ControllersTests.Admin
{
    public abstract class AdminTest: BaseControllerTest
    {
        [SetUp]
        public async new Task Setup()
        {
            await _authServiceHelper.LoginAsAdmin();
        }
    }
}

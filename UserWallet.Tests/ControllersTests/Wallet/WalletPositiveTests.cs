namespace UserWallet.Tests.ControllersTests.Wallet
{
    public class WalletPositiveTests: BaseControllerTest
    {
        private AuthServiceHelper _authServiceHelper;
        private TransactionServiceHelper _transactionServiceHelper;

        [SetUp]
        public async Task Setup()
        {
            await base.Setup();

        }

        [Test]
        public async Task PositiveGetUserBalanceTest()
        {

        }
    }
}

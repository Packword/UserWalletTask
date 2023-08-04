namespace UserWallet.Tests.ControllersTests.Wallet
{
    public class WalletPositiveTests: BaseControllerTest
    {
        private TransactionServiceHelper _transactionServiceHelper;

        [SetUp]
        public new void Setup()
        {
            _transactionServiceHelper = new TransactionServiceHelper(_client);
        }

        [Test]
        public async Task PositiveGetUserBalanceTest()
        {
            await _transactionServiceHelper.CreateFirstDepositAndApprove("btc", 50, "1234567890123456");
            await _authServiceHelper.LoginAsUser();

            var userBalance = await GetUserBalance();

            userBalance!["btc"].Amount.Should().Be(50);
        }

        [Test]
        public async Task PositiveGetCurrentUserDepositsTest()
        {
            await _transactionServiceHelper.CreateCryptoDeposit("btc", 50, "1234567890123456");
            await _authServiceHelper.LoginAsUser();
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "wallet/tx");
            var response = await _client.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();
            var deposits = JsonSerializer.Deserialize<List<Deposit>>(content, TestData.JSON_OPTIONS);

            deposits!.Count.Should().Be(1);
            deposits[0].Amount.Should().Be(50);
            deposits[0].CurrencyId.Should().Be("btc");
        }

        [Test]
        public async Task PositiveGetUserDepositsTest()
        {
            await _transactionServiceHelper.CreateFirstDepositAndApprove("btc", 50, "1234567890123456");
            await _authServiceHelper.LoginAsAdmin();

            var userBalance = await GetUserBalance(TestData.DEFAULT_USER_ID);

            userBalance!["btc"].Amount.Should().Be(50);
        }

        [Test]
        public async Task PositiveCreateDepositTest()
        {
            var responseCrypto = await _transactionServiceHelper.CreateCryptoDeposit("btc", 50, "1234567890123456");
            var responseFiat = await _transactionServiceHelper.CreateFiatDeposit("rub", 50, "1234567890123456", "1234567890123456");

            responseFiat.StatusCode.Should().Be(HttpStatusCode.OK);
            responseCrypto.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        private async Task<Dictionary<string, BalanceDTO>?> GetUserBalance(int id = -1)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, id == -1 ? "wallet/balance" : $"wallet/{id}");
            var response = await _client.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Dictionary<string, BalanceDTO>>(content, TestData.JSON_OPTIONS);
        }
    }
}

namespace UserWallet.Tests.ControllersTests.Admin
{
    public class AdminWalletTransactionsPositiveTests: AdminTest
    {
        private TransactionServiceHelper _transactionServiceHelper;

        [SetUp]
        public async override Task Setup()
        {
            await base.Setup();
            _transactionServiceHelper = new TransactionServiceHelper(_client);
        }

        [Test]
        public async Task PositiveGetTransactionsTest()
        {
            await CreateTestTransaction();
            var transactions = await GetTransactions();
            transactions!.Count.Should().Be(1);
            transactions.First(u => u.Id == 1).Status.Should().Be(DepositStatus.Undecided);
        }

        [Test]
        public async Task PositiveApproveTransactionTest()
        {
            await CreateTestTransaction();
            var response = await ApproveTransaction(1);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var transactions = await GetTransactions();
            transactions!.First(u => u.Id == 1).Status.Should().Be(DepositStatus.Approved);
        }

        [Test]
        public async Task PositiveDeclineTransactionTest()
        {
            await CreateTestTransaction();
            var response = await DeclineTransaction(1);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var transactions = await GetTransactions();
            transactions!.First(u => u.Id == 1).Status.Should().Be(DepositStatus.Declined);
        }

        private async Task<HttpResponseMessage> DeclineTransaction(int txId)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"admin/wallet/tx/decline/{txId}");
            return await _client.SendAsync(requestMessage);
        }

        private async Task<HttpResponseMessage> ApproveTransaction(int txId)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"admin/wallet/tx/approve/{txId}");
            return await _client.SendAsync(requestMessage);
        }

        private async Task<List<Deposit>?> GetTransactions()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "admin/wallet/tx");
            var response = await _client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            string content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<Deposit>?>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result;
        }

        private async Task CreateTestTransaction()
        {
            await _authServiceHelper.Logout();
            await _authServiceHelper.LoginAsUser();
            await _transactionServiceHelper.CreateCryptoDeposit("btc", 50, "1234567890123456");
            await _authServiceHelper.Logout();
            await _authServiceHelper.LoginAsAdmin();
        }
    }
}

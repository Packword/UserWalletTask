namespace UserWallet.Tests.ControllersTests.Admin
{
    public class AdminWalletTransactionsPositiveTests: BaseControllerTest
    {
        private TransactionServiceHelper _transactionServiceHelper;
        private BalanceServiceHelper _balanceServiceHelper;
        private const int TEST_TRANSACTION_ID = 1;

        [SetUp]
        public new void Setup()
        {
            _transactionServiceHelper = new TransactionServiceHelper(_client);
            _balanceServiceHelper = new BalanceServiceHelper(_client);
        }

        [Test]
        public async Task GetTransactions_AsAdmin_Success()
        {
            await _authServiceHelper.LoginAsAdmin();

            var response = await SendGetTransactionsRequest();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task GetTransactions_AsAdmin_ReturnedTransactionsCorrect()
        {
            await _authServiceHelper.LoginAsAdmin();
            await CreateTestCryptoTransaction();

            var transactions = await GetTransactions();

            transactions!.Count.Should().Be(TEST_TRANSACTION_ID);
            transactions.First(u => u.Id == TEST_TRANSACTION_ID).Status.Should().Be(DepositStatus.Undecided);
        }

        [Test]
        public async Task ApproveTransaction_AsAdmin_Success()
        {
            await _authServiceHelper.LoginAsAdmin();
            await CreateTestCryptoTransaction();

            var response = await ApproveTransaction(TEST_TRANSACTION_ID);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task ApproveTransaction_AsAdmin_TransactionHasBecomeApproved()
        {
            await _authServiceHelper.LoginAsAdmin();
            await CreateTestCryptoTransaction();

            await ApproveTransaction(TEST_TRANSACTION_ID);
            var transactions = await GetTransactions();

            transactions!.First(u => u.Id == TEST_TRANSACTION_ID).Status.Should().Be(DepositStatus.Approved);
        }

        [Test]
        public async Task ApproveTransaction_AsAdmin_BalanceHasChanged()
        {
            await _authServiceHelper.LoginAsAdmin();
            await CreateTestCryptoTransaction();

            await ApproveTransaction(TEST_TRANSACTION_ID);
            var userBalances = await _balanceServiceHelper.GetUserBalance(TestData.DEFAULT_USER_ID);

            userBalances![TestData.CRYPTO_CURRENCY_ID].Amount.Should().Be(TestData.DEFAULT_DEPOSIT_AMOUNT);
        }

        [Test]
        public async Task DeclineTransaction_AsAdmin_Success()
        {
            await _authServiceHelper.LoginAsAdmin();
            await CreateTestCryptoTransaction();

            var response = await DeclineTransaction(TEST_TRANSACTION_ID);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task DeclineTransaction_AsAdmin_TransactionHasBecomeDeclined()
        {
            await _authServiceHelper.LoginAsAdmin();
            await CreateTestCryptoTransaction();

            await DeclineTransaction(TEST_TRANSACTION_ID);
            var transactions = await GetTransactions();

            transactions!.First(u => u.Id == TEST_TRANSACTION_ID).Status.Should().Be(DepositStatus.Declined);
        }

        [Test]
        public async Task DeclineTransaction_AsAdmin_BalanceHasNotChanged()
        {
            await _authServiceHelper.LoginAsAdmin();
            await CreateTestCryptoTransaction();

            await DeclineTransaction(TEST_TRANSACTION_ID);
            var userBalances = await _balanceServiceHelper.GetUserBalance(TestData.DEFAULT_USER_ID);

            userBalances!.ContainsKey(TestData.CRYPTO_CURRENCY_ID).Should().Be(false);
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
            var response = await SendGetTransactionsRequest();
            string content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<Deposit>?>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result;
        }

        private async Task<HttpResponseMessage> SendGetTransactionsRequest()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "admin/wallet/tx");
            return await _client.SendAsync(requestMessage);
        }

        private async Task CreateTestCryptoTransaction()
        {
            await _transactionServiceHelper.CreateCryptoDepositAsDefaultUser(TestData.CRYPTO_CURRENCY_ID,
                                                                TestData.DEFAULT_DEPOSIT_AMOUNT,
                                                                TestData.CRYPTO_ADDRESS);
            await _authServiceHelper.LoginAsAdmin();
        }
    }
}

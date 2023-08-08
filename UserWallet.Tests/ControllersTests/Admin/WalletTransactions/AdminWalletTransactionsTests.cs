namespace UserWallet.Tests.ControllersTests.Admin
{
    public class AdminWalletTransactionsTests: BaseControllerTest
    {
        private const int TEST_TRANSACTION_ID = 1;
        private const int TEST_TRANSACTION_COUNT = 1;

        [SetUp]
        public new void Setup()
        {
        }

        [Test]
        public async Task GetTransactions_AsAdmin_Success()
        {
            await LoginAsAdmin(_client);    

            var response = await _client.GetAsync("admin/wallet/tx");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task GetTransactions_AsAdmin_ReturnedTransactionsCorrect()
        {
            using var userClient = new HttpClient();
            await LoginAsAdmin(_client);
            await LoginAsUser(userClient);
            await CreateTestCryptoTransaction(userClient);

            var transactions = await _client.GetTransactions();

            transactions.Should().NotBeNull().And.HaveCount(TEST_TRANSACTION_COUNT);
            var transaction = transactions!.FirstOrDefault(u => u.Id == TEST_TRANSACTION_ID);
            transaction.Should().NotBeNull();
            transaction!.Status.Should().Be(DepositStatus.Undecided);
        }

        [Test]
        public async Task ApproveTransaction_AsAdmin_Success()
        {
            using var userClient = new HttpClient();
            await LoginAsAdmin(_client);
            await LoginAsUser(userClient);
            await CreateTestCryptoTransaction(userClient);

            var response = await _client.ApproveTransaction(TEST_TRANSACTION_ID);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task ApproveTransaction_AsAdmin_TransactionHasBecomeApproved()
        {
            using var userClient = new HttpClient();
            await LoginAsAdmin(_client);
            await LoginAsUser(userClient);
            await CreateTestCryptoTransaction(userClient);

            await _client.ApproveTransaction(TEST_TRANSACTION_ID);
            var transactions = await _client.GetTransactions();

            transactions.Should().NotBeNull().And.HaveCount(TEST_TRANSACTION_COUNT);
            var transaction = transactions!.FirstOrDefault(u => u.Id == TEST_TRANSACTION_ID);
            transaction.Should().NotBeNull();
            transaction!.Status.Should().Be(DepositStatus.Approved);
        }

        [Test]
        public async Task ApproveTransaction_AsAdmin_BalanceHasChanged()
        {
            using var userClient = new HttpClient();
            await LoginAsAdmin(_client);
            await LoginAsUser(userClient);
            await CreateTestCryptoTransaction(userClient);

            await _client.ApproveTransaction(TEST_TRANSACTION_ID);
            var userBalances = await _client.GetUserBalance(DEFAULT_USER_ID);

            userBalances.Should().NotBeNull().And.ContainKey(CRYPTO_CURRENCY_ID);
            userBalances![CRYPTO_CURRENCY_ID].Amount.Should().Be(DEFAULT_DEPOSIT_AMOUNT);
        }

        [Test]
        public async Task DeclineTransaction_AsAdmin_Success()
        {
            using var userClient = new HttpClient();
            await LoginAsAdmin(_client);
            await LoginAsUser(userClient);
            await CreateTestCryptoTransaction(userClient);

            var response = await _client.DeclineTransaction(TEST_TRANSACTION_ID);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task DeclineTransaction_AsAdmin_TransactionHasBecomeDeclined()
        {
            using var userClient = new HttpClient();
            await LoginAsAdmin(_client);
            await LoginAsUser(userClient);
            await CreateTestCryptoTransaction(userClient);

            await _client.DeclineTransaction(TEST_TRANSACTION_ID);
            var transactions = await _client.GetTransactions();

            transactions.Should().NotBeNull();
            var transaction = transactions!.FirstOrDefault(u => u.Id == TEST_TRANSACTION_ID);
            transaction.Should().NotBeNull();
            transaction!.Status.Should().Be(DepositStatus.Declined);
        }

        [Test]
        public async Task DeclineTransaction_AsAdmin_BalanceHasNotChanged()
        {
            using var userClient = new HttpClient();
            await LoginAsAdmin(_client);
            await LoginAsUser(userClient);
            await CreateTestCryptoTransaction(userClient);

            await _client.DeclineTransaction(TEST_TRANSACTION_ID);
            var userBalances = await _client.GetUserBalance(DEFAULT_USER_ID);

            userBalances.Should().NotBeNull().And.NotContainKey(CRYPTO_CURRENCY_ID);
        }

        private async Task CreateTestCryptoTransaction(HttpClient client)
           => await client.CreateDeposit(CRYPTO_CURRENCY_ID,
                                          TransactionServiceHelper.CreateDepositDTO(
                                              DEFAULT_DEPOSIT_AMOUNT,
                                              CRYPTO_ADDRESS,
                                              null,
                                              null
                                          ));
    }
}

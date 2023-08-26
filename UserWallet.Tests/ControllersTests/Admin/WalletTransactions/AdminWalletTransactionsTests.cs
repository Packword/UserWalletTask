namespace UserWallet.Tests.ControllersTests.Admin
{
    public class AdminWalletTransactionsTests: BaseControllerTest
    {
        private const int TEST_TRANSACTION_ID = 1;
        private const int TEST_TRANSACTION_COUNT = 1;
        private const int NON_EXISTENT_TRANSACTION_ID = -1;

        [TestCaseSource(nameof(AccessGetTransactionsData))]
        public async Task GetTransactions_DifferentUsers_CorrectAnswer(string? username, string? password, HttpStatusCode expectedResult)
        {
            await Client.Login(username, password);

            var response = await Client.GetTransactionsResponse();

            response.StatusCode.Should().Be(expectedResult);
        }

        [Test]
        public async Task GetTransactions_OneTransaction_ReturnedTransactionsCorrect()
        {
            await LoginAsAdmin(Client);
            await CreateUserClientAndCreateDeposit(CRYPTO_CURRENCY_ID, DEFAULT_DEPOSIT_AMOUNT, CRYPTO_ADDRESS);

            var transactions = await Client.GetTransactions();

            transactions.Should().NotBeNull().And.HaveCount(TEST_TRANSACTION_COUNT);
            var transaction = transactions!.FirstOrDefault(u => u.Id == TEST_TRANSACTION_ID);
            transaction.Should().NotBeNull();
            transaction!.Status.Should().Be(DepositStatus.Undecided);
        }

        [Test]
        public async Task GetTransactions_ZeroTransactions_ReturnedEmptyList()
        {
            await LoginAsAdmin(Client);

            var transactions = await Client.GetTransactions();

            transactions.Should().NotBeNull().And.BeEmpty();
        }

        [TestCaseSource(nameof(DecideTransactionData))]
        public async Task ApproveTransaction_DifferentUsers_CorrectAnswer(string? username, string? password, string? transactionId, HttpStatusCode expectedResult)
        {
            await Client.Login(username, password);
            await CreateUserClientAndCreateDeposit(CRYPTO_CURRENCY_ID, DEFAULT_DEPOSIT_AMOUNT, CRYPTO_ADDRESS);

            var response = await Client.ApproveTransaction(transactionId);

            response.StatusCode.Should().Be(expectedResult);
        }

        [Test]
        public async Task ApproveTransaction_AsAdmin_TransactionHasBecomeApproved()
        {
            await LoginAsAdmin(Client);
            await CreateUserClientAndCreateDeposit(CRYPTO_CURRENCY_ID, DEFAULT_DEPOSIT_AMOUNT, CRYPTO_ADDRESS);

            await Client.ApproveTransaction(TEST_TRANSACTION_ID.ToString());
            var transactions = await Client.GetTransactions();

            transactions.Should().NotBeNull().And.HaveCount(TEST_TRANSACTION_COUNT);
            var transaction = transactions!.FirstOrDefault(u => u.Id == TEST_TRANSACTION_ID);
            transaction.Should().NotBeNull();
            transaction!.Status.Should().Be(DepositStatus.Approved);
        }

        [Test]
        public async Task ApproveTransaction_AsAdmin_BalanceHasChanged()
        {
            await LoginAsAdmin(Client);
            await CreateUserClientAndCreateDeposit(CRYPTO_CURRENCY_ID, DEFAULT_DEPOSIT_AMOUNT, CRYPTO_ADDRESS);

            await Client.ApproveTransaction(TEST_TRANSACTION_ID.ToString());
            var userBalances = await Client.GetUserBalance(DEFAULT_USER_ID);

            userBalances.Should().NotBeNull().And.ContainKey(CRYPTO_CURRENCY_ID);
            userBalances![CRYPTO_CURRENCY_ID].Amount.Should().Be(DEFAULT_DEPOSIT_AMOUNT);
        }

        [Test]
        public async Task ApproveTransaction_AlreadyApproved_BadRequestAndBalanceHasNotBeDoubled()
        {
            await LoginAsAdmin(Client);
            await CreateUserClientAndCreateDeposit(CRYPTO_CURRENCY_ID, DEFAULT_DEPOSIT_AMOUNT, CRYPTO_ADDRESS);
            await Client.ApproveTransaction(TEST_TRANSACTION_ID.ToString());

            var response = await Client.ApproveTransaction(TEST_TRANSACTION_ID.ToString());
            var userBalances = await Client.GetUserBalance(DEFAULT_USER_ID);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            userBalances.Should().NotBeNull().And.ContainKey(CRYPTO_CURRENCY_ID);
            userBalances![CRYPTO_CURRENCY_ID].Amount.Should().Be(DEFAULT_DEPOSIT_AMOUNT);

        }

        [Test]
        public async Task ApproveTransaction_AlreadyDeclined_BadRequest()
        {
            await LoginAsAdmin(Client);
            await CreateUserClientAndCreateDeposit(CRYPTO_CURRENCY_ID, DEFAULT_DEPOSIT_AMOUNT, CRYPTO_ADDRESS);
            await Client.DeclineTransaction(TEST_TRANSACTION_ID.ToString());

            var response = await Client.ApproveTransaction(TEST_TRANSACTION_ID.ToString());
            var transactions = await Client.GetTransactions();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            transactions.Should().NotBeNull();
            var transaction = transactions!.FirstOrDefault(u => u.Id == TEST_TRANSACTION_ID);
            transaction.Should().NotBeNull();
            transaction!.Status.Should().Be(DepositStatus.Declined);
        }

        [TestCaseSource(nameof(DecideTransactionData))]
        public async Task DeclineTransaction_DifferentUsers_CorrectAnswer(string? username, string? password, string? transactionId, HttpStatusCode expectedResult)
        {
            await Client.Login(username, password);
            await CreateUserClientAndCreateDeposit(CRYPTO_CURRENCY_ID, DEFAULT_DEPOSIT_AMOUNT, CRYPTO_ADDRESS);

            var response = await Client.DeclineTransaction(transactionId);

            response.StatusCode.Should().Be(expectedResult);
        }

        [Test]
        public async Task DeclineTransaction_AsAdmin_TransactionHasBecomeDeclined()
        {
            await LoginAsAdmin(Client);
            await CreateUserClientAndCreateDeposit(CRYPTO_CURRENCY_ID, DEFAULT_DEPOSIT_AMOUNT, CRYPTO_ADDRESS);

            await Client.DeclineTransaction(TEST_TRANSACTION_ID.ToString());
            var transactions = await Client.GetTransactions();

            transactions.Should().NotBeNull();
            var transaction = transactions!.FirstOrDefault(u => u.Id == TEST_TRANSACTION_ID);
            transaction.Should().NotBeNull();
            transaction!.Status.Should().Be(DepositStatus.Declined);
        }

        [Test]
        public async Task DeclineTransaction_AsAdmin_BalanceHasNotChanged()
        {
            await LoginAsAdmin(Client);
            await CreateUserClientAndCreateDeposit(CRYPTO_CURRENCY_ID, DEFAULT_DEPOSIT_AMOUNT, CRYPTO_ADDRESS);

            await Client.DeclineTransaction(TEST_TRANSACTION_ID.ToString());
            var userBalances = await Client.GetUserBalance(DEFAULT_USER_ID);

            userBalances.Should().NotBeNull().And.NotContainKey(CRYPTO_CURRENCY_ID);
        }

        [Test]
        public async Task DeclineTransaction_AlreadyApproved_BadRequestAndStatusHasNotChanged()
        {
            await LoginAsAdmin(Client);
            await CreateUserClientAndCreateDeposit(CRYPTO_CURRENCY_ID, DEFAULT_DEPOSIT_AMOUNT, CRYPTO_ADDRESS);
            await Client.ApproveTransaction(TEST_TRANSACTION_ID.ToString());

            var response = await Client.DeclineTransaction(TEST_TRANSACTION_ID.ToString());
            var transactions = await Client.GetTransactions();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            transactions.Should().NotBeNull();
            var transaction = transactions!.FirstOrDefault(u => u.Id == TEST_TRANSACTION_ID);
            transaction.Should().NotBeNull();
            transaction!.Status.Should().Be(DepositStatus.Approved);
        }

        [Test]
        public async Task DeclineTransaction_AlreadyDeclined_BadRequest()
        {
            await LoginAsAdmin(Client);
            await CreateUserClientAndCreateDeposit(CRYPTO_CURRENCY_ID, DEFAULT_DEPOSIT_AMOUNT, CRYPTO_ADDRESS);
            await Client.DeclineTransaction(TEST_TRANSACTION_ID.ToString());

            var response = await Client.DeclineTransaction(TEST_TRANSACTION_ID.ToString());

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        private static IEnumerable<TestCaseData> AccessGetTransactionsData()
        {
            yield return new TestCaseData(ADMIN_USERNAME, ADMIN_PASSWORD, HttpStatusCode.OK);
            yield return new TestCaseData(DEFAULT_USER_USERNAME, DEFAULT_USER_PASSWORD, HttpStatusCode.Forbidden);
            yield return new TestCaseData(null, null, HttpStatusCode.Unauthorized);
        }

        private static IEnumerable<TestCaseData> DecideTransactionData()
        {
            yield return new TestCaseData(ADMIN_USERNAME, ADMIN_PASSWORD, TEST_TRANSACTION_ID.ToString(), HttpStatusCode.OK);
            yield return new TestCaseData(DEFAULT_USER_USERNAME, DEFAULT_USER_PASSWORD, TEST_TRANSACTION_ID.ToString(), HttpStatusCode.Forbidden);
            yield return new TestCaseData(null, null, TEST_TRANSACTION_ID.ToString(), HttpStatusCode.Unauthorized);
            yield return new TestCaseData(ADMIN_USERNAME, ADMIN_PASSWORD, NON_EXISTENT_TRANSACTION_ID.ToString(), HttpStatusCode.NotFound);
            yield return new TestCaseData(ADMIN_USERNAME, ADMIN_PASSWORD, "adasf", HttpStatusCode.NotFound);

        }

        private async Task CreateUserClientAndCreateDeposit(string currecnyId, decimal amount, string address)
        {
            using var userClient = Factory.CreateClient();
            await LoginAsUser(userClient);
            await CreateCryptoTransaction(userClient, currecnyId, amount, address);
        }

        private async static Task CreateCryptoTransaction(HttpClient client, string currecnyId, decimal amount, string address)
           => await client.CreateDeposit(currecnyId,
                                          new DepositDTO(
                                              amount,
                                              address,
                                              null,
                                              null
                                          ));
    }
}

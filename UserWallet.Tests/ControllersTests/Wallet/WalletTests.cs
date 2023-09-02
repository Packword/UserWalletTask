namespace UserWallet.Tests.ControllersTests.Wallet
{
    public class WalletTests: BaseControllerTest
    {
        private const int TEST_TRANSACTION_ID = 1;
        private const int TEST_TRANSACTION_COUNT = 1;
        private const string TEST_USERNAME = "Test";
        private const string TEST_PASSWORD = "ForTest";
        private const int NON_EXISTENT_USER_ID = 0;


        [TestCaseSource(nameof(AccessCurrentUserData))]
        public async Task<HttpStatusCode> GetCurrentUserBalance_DifferentUsers_CorrectAnswer(string? username, string? password)
        {
            await Client.Login(username, password);

            var response = await Client.GetCurrentUserBalanceResponse();

            return response.StatusCode;
        }

        [Test]
        public async Task GetCurrentUserBalance_WithOneDeposit_ReturnedBalanceIsCorrect()
        {
            await LoginAsUser(Client);
            var userBalanceBefore = await Client.GetCurrentUserBalance();
            await CreateCryptoDeposit(Client);
            using var adminClient = Factory.CreateClient();
            await LoginAsAdmin(adminClient);
            await adminClient.ApproveTransaction(TEST_TRANSACTION_ID.ToString());

            var userBalanceAfter = await Client.GetCurrentUserBalance();

            userBalanceAfter.Should().NotBeNull().And.ContainKey(CRYPTO_CURRENCY_ID);
            var difference = userBalanceAfter![CRYPTO_CURRENCY_ID].Amount - (userBalanceBefore!.GetValueOrDefault(CRYPTO_CURRENCY_ID)?.Amount ?? 0);
            difference.Should().Be(DEFAULT_DEPOSIT_AMOUNT);
        }

        [Test]
        public async Task GetCurrentUserBalance_WithoutDeposits_ReturnedEmptyBalance()
        {
            await LoginAsUser(Client);

            var userBalance = await Client.GetCurrentUserBalance();

            userBalance.Should().NotBeNull().And.BeEmpty();
        }

        [TestCaseSource(nameof(AccessCurrentUserData))]
        public async Task<HttpStatusCode> GetCurrentUserDeposits_DifferentUsers_CorrectAnswer(string? username, string? password)
        {
            await Client.Login(username, password);

            var response = await Client.GetCurrentUserDepositsResponse();

            return response.StatusCode;
        }

        [Test]
        public async Task GetCurrentUserDeposits_WithouDeposits_ReturnedEmptyList()
        {
            await LoginAsUser(Client);

            var deposits = await Client.GetCurrentUserDeposits();

            deposits.Should().NotBeNull().And.BeEmpty();
        }

        [Test]
        public async Task GetCurrentUserDeposits_WithOneCryptoDeposit_ReturnedDepositsCorrect()
        {
            await LoginAsUser(Client);
            await CreateCryptoDeposit(Client);

            var deposits = await Client.GetCurrentUserDeposits();

            deposits.Should().NotBeNull().And.HaveCount(TEST_TRANSACTION_COUNT);
            deposits![0].Amount.Should().Be(DEFAULT_DEPOSIT_AMOUNT);
            deposits[0].CurrencyId.Should().Be(CRYPTO_CURRENCY_ID);
            deposits[0].Status.Should().Be(DepositStatus.Undecided);
        }

        [Test]
        public async Task GetCurrentUserDeposits_WithOtherUserDeposit_ReturnedEmptyList()
        {
            await LoginAsUser(Client);
            await Client.SignUp(TEST_USERNAME, TEST_PASSWORD);
            using var secondClient = Factory.CreateClient();
            await secondClient.Login(TEST_USERNAME, TEST_PASSWORD);
            await CreateCryptoDeposit(secondClient);

            var currentUserDeposits = await Client.GetCurrentUserDeposits();
            var otherUserDeposits = await secondClient.GetCurrentUserDeposits();

            currentUserDeposits.Should().NotBeNull().And.BeEmpty();
            otherUserDeposits.Should().NotBeNull().And.HaveCount(1);
        }

        [TestCaseSource(nameof(AccessGetUserBalancesData))]
        public async Task<HttpStatusCode> GetUserBalances_DifferentUsers_CorrectAnswer(string? username, string? password, string? userId)
        {
            await Client.Login(username, password);

            var response = await Client.GetUserBalanceResponse(userId);

            return response.StatusCode;
        }

        [Test]
        public async Task GetUserBalances_AfterDeposit_ReturnedBalancesIsCorrect()
        {
            var userClient = Factory.CreateClient();
            await LoginAsUser(userClient);
            await CreateCryptoDeposit(userClient);
            await LoginAsAdmin(Client);
            await Client.ApproveTransaction(TEST_TRANSACTION_ID.ToString());

            var userBalance = await Client.GetUserBalance(DEFAULT_USER_ID);

            userBalance.Should().NotBeNull().And.ContainKey(CRYPTO_CURRENCY_ID);
            userBalance![CRYPTO_CURRENCY_ID].Amount.Should().Be(DEFAULT_DEPOSIT_AMOUNT);
        }

        [Test]
        public async Task GetUserBalances_WithoutDeposit_ReturnedEmptyList()
        {
            await LoginAsAdmin(Client);

            var userBalance = await Client.GetUserBalance(DEFAULT_USER_ID);

            userBalance.Should().NotBeNull().And.BeEmpty();
        }

        [TestCaseSource(nameof(AccessCurrentUserData))]
        public async Task<HttpStatusCode> GetUserBalances_DifferentUsers_CorrectAnswer(string? username, string? password)
        {
            await Client.Login(username, password);

            var response = await CreateCryptoDeposit(Client);

            return response.StatusCode;
        }

        [TestCaseSource(nameof(CreateDepositCorrectData))]
        public async Task CreateDeposit_WithCorrectData_DepositHasCreated(string currencyId, DepositDTO deposit)
        {
            await LoginAsUser(Client);

            await Client.CreateDeposit(currencyId, deposit);
            var deposits = await Client.GetCurrentUserDeposits();

            deposits.Should().NotBeNull().And.HaveCount(1);
            deposits!.Count.Should().Be(TEST_TRANSACTION_COUNT);
            deposits[0].Amount.Should().Be(deposit.Amount);
            deposits[0].CurrencyId.Should().Be(currencyId);
            deposits[0].Status.Should().Be(DepositStatus.Undecided);
        }

        [TestCaseSource(nameof(CreateDepositIncorrectData))]
        public async Task CreateDeposit_WithIncorrectData_BadRequestAndDepositHasNotCreated(string currencyId, DepositDTO deposit)
        {
            await LoginAsUser(Client);

            var response = await Client.CreateDeposit(currencyId, deposit);
            var deposits = await Client.GetCurrentUserDeposits();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            deposits.Should().NotBeNull().And.BeEmpty();
        }

        private static IEnumerable<TestCaseData> AccessCurrentUserData()
        {
            yield return new TestCaseData(ADMIN_USERNAME, ADMIN_PASSWORD).Returns(HttpStatusCode.Forbidden);
            yield return new TestCaseData(DEFAULT_USER_USERNAME, DEFAULT_USER_PASSWORD).Returns(HttpStatusCode.OK);
            yield return new TestCaseData(null, null).Returns(HttpStatusCode.Unauthorized);
        }

        private static IEnumerable<TestCaseData> AccessGetUserBalancesData()
        {
            yield return new TestCaseData(ADMIN_USERNAME, ADMIN_PASSWORD, DEFAULT_USER_ID.ToString()).Returns(HttpStatusCode.OK);
            yield return new TestCaseData(DEFAULT_USER_USERNAME, DEFAULT_USER_PASSWORD, DEFAULT_USER_ID.ToString()).Returns(HttpStatusCode.Forbidden);
            yield return new TestCaseData(null, null, DEFAULT_USER_ID.ToString()).Returns(HttpStatusCode.Unauthorized);
            yield return new TestCaseData(ADMIN_USERNAME, ADMIN_PASSWORD, NON_EXISTENT_USER_ID.ToString()).Returns(HttpStatusCode.NotFound);
            yield return new TestCaseData(ADMIN_USERNAME, ADMIN_PASSWORD, "asdfg").Returns(HttpStatusCode.NotFound);
        }

        private static IEnumerable<TestCaseData> CreateDepositCorrectData()
        {
            yield return new TestCaseData(CRYPTO_CURRENCY_ID, new DepositDTO(DEFAULT_DEPOSIT_AMOUNT, CRYPTO_ADDRESS, null, null));
            yield return new TestCaseData(CRYPTO_CURRENCY_ID, new DepositDTO(DEFAULT_DEPOSIT_AMOUNT, CRYPTO_ADDRESS, FIAT_CARDHOLDER, FIAT_CARDNUMBER));
            yield return new TestCaseData(CRYPTO_CURRENCY_ID, new DepositDTO(DEFAULT_DEPOSIT_AMOUNT, CRYPTO_ADDRESS, "123", "123"));
            yield return new TestCaseData(FIAT_CURRENCY_ID, new DepositDTO(DEFAULT_DEPOSIT_AMOUNT, null, FIAT_CARDHOLDER, FIAT_CARDNUMBER));
            yield return new TestCaseData(FIAT_CURRENCY_ID, new DepositDTO(DEFAULT_DEPOSIT_AMOUNT, "123", FIAT_CARDHOLDER, FIAT_CARDNUMBER));
            yield return new TestCaseData(FIAT_CURRENCY_ID, new DepositDTO(DEFAULT_DEPOSIT_AMOUNT, CRYPTO_ADDRESS, FIAT_CARDHOLDER, FIAT_CARDNUMBER));
        }

        private static IEnumerable<TestCaseData> CreateDepositIncorrectData()
        {
            yield return new TestCaseData(
                CRYPTO_CURRENCY_ID, 
                new DepositDTO(0, CRYPTO_ADDRESS, null, null)
            );
            yield return new TestCaseData(
                CRYPTO_CURRENCY_ID,
                new DepositDTO(-5, CRYPTO_ADDRESS, null, null)
            );
            yield return new TestCaseData(
                CRYPTO_CURRENCY_ID,
                new DepositDTO(200, CRYPTO_ADDRESS, null, null)
            );
            yield return new TestCaseData(
                CRYPTO_CURRENCY_ID,
                new DepositDTO(DEFAULT_DEPOSIT_AMOUNT, "123", null, null)
            );
            yield return new TestCaseData(
                 CRYPTO_CURRENCY_ID,
                 new DepositDTO(DEFAULT_DEPOSIT_AMOUNT, "12345678901234567890", null, null)
            );
            yield return new TestCaseData(
                 CRYPTO_CURRENCY_ID,
                 new DepositDTO(DEFAULT_DEPOSIT_AMOUNT, null, null, null)
            );
            yield return new TestCaseData(
                "asdfasdgasd",
                new DepositDTO(0, null, null, null)
            );
            yield return new TestCaseData(
                FIAT_CURRENCY_ID,
                new DepositDTO(DEFAULT_DEPOSIT_AMOUNT, null, "1", FIAT_CARDNUMBER)
            );
            yield return new TestCaseData(
                FIAT_CURRENCY_ID,
                new DepositDTO(DEFAULT_DEPOSIT_AMOUNT, null, "12345678901234567890", FIAT_CARDNUMBER)
            );
            yield return new TestCaseData(
                FIAT_CURRENCY_ID,
                new DepositDTO(DEFAULT_DEPOSIT_AMOUNT, null, FIAT_CARDHOLDER, "1234")
            );
        }

        private async static Task<HttpResponseMessage> CreateCryptoDeposit(HttpClient client)
        {
            var deposit = new DepositDTO(DEFAULT_DEPOSIT_AMOUNT, CRYPTO_ADDRESS, null, null);
            return await client.CreateDeposit(CRYPTO_CURRENCY_ID, deposit);
        }
    }
}

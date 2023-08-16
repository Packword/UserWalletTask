namespace UserWallet.Tests.ControllersTests.Wallet
{
    public class WalletTests: BaseControllerTest
    {
        private const int TEST_TRANSACTION_ID = 1;
        private const int TEST_TRANSACTION_COUNT = 1;

        [Test]
        public async Task GetCurrentUserBalance_AsUser_Success()
        {
            await LoginAsUser(Client);

            var response = await Client.GetAsync("wallet/balance");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task GetCurrentUserBalance_AsAnonymous_Unauthorized()
        {
            var response = await Client.GetAsync("wallet/balance");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task GetCurrentUserBalance_AsAdmin_Forbidden()
        {
            await LoginAsAdmin(Client);

            var response = await Client.GetAsync("wallet/balance");

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Test]
        public async Task GetCurrentUserBalance_WithOneDeposit_ReturnedBalanceIsCorrect()
        {
            await LoginAsUser(Client);
            await CreateCryptoDeposit(Client);
            await CreateAdminClientAndApproveTransaction(TEST_TRANSACTION_ID);

            var userBalance = await Client.GetCurrentUserBalance();

            userBalance.Should().NotBeNull().And.ContainKey(CRYPTO_CURRENCY_ID);
            userBalance![CRYPTO_CURRENCY_ID].Amount.Should().Be(DEFAULT_DEPOSIT_AMOUNT);
        }

        [Test]
        public async Task GetCurrentUserBalance_WithoutDeposits_ReturnedEmptyBalance()
        {
            await LoginAsUser(Client);

            var userBalance = await Client.GetCurrentUserBalance();

            userBalance.Should().NotBeNull().And.BeEmpty();
        }

        [Test]
        public async Task GetCurrentUserDeposits_AsUser_Success()
        {
            await LoginAsUser(Client);

            var response = await Client.GetAsync("wallet/tx");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task GetCurrentUserDeposits_AsAdmin_Forbidden()
        {
            await LoginAsAdmin(Client);

            var response = await Client.GetAsync("wallet/tx");

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Test]
        public async Task GetCurrentUserDeposits_AsAnonymous_Unauthorized()
        {
            var response = await Client.GetAsync("wallet/tx");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
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
            await Client.SignUp("Test", "ForTest");
            using var secondClient = Factory.CreateClient();
            await secondClient.Login("Test", "ForTest");
            await CreateCryptoDeposit(secondClient);

            var deposits = await Client.GetCurrentUserDeposits();

            deposits.Should().NotBeNull().And.BeEmpty();
        }

        [Test]
        public async Task GetUserBalances_AsAdmin_Success()
        {
            await LoginAsAdmin(Client);

            var response = await Client.GetAsync($"wallet/{DEFAULT_USER_ID}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task GetUserBalances_AsUser_Forbidden()
        {
            await LoginAsUser(Client);

            var response = await Client.GetAsync($"wallet/{DEFAULT_USER_ID}");

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Test]
        public async Task GetUserBalances_AsAnonymous_Unauthorized()
        {
            var response = await Client.GetAsync($"wallet/{DEFAULT_USER_ID}");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task GetUserBalances_AfterDeposit_ReturnedBalancesIsCorrect()
        {
            await CreateUserClientAndCreateCryptoDeposit();
            await LoginAsAdmin(Client);
            await Client.ApproveTransaction(TEST_TRANSACTION_ID);

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

        [Test]
        public async Task CreateDeposit_AsAnonymous_Unauthorized()
        {
            var response = await CreateCryptoDeposit(Client);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task CreateDeposit_AsAdmin_Forbidden()
        {
            await LoginAsAdmin(Client);

            var response = await CreateCryptoDeposit(Client);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Test]
        public async Task CreateDeposit_AsUser_Success()
        {
            await LoginAsUser(Client);

            var response = await CreateCryptoDeposit(Client);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestCaseSource(nameof(CreateDepositCorrectData))]
        public async Task CreateDeposit_WithCorrectData_DepositHasCreated(string currencyId, DepositDTO deposit)
        {
            await LoginAsUser(Client);


            await Client.PutAsJsonAsync($"/wallet/deposit/{currencyId}", deposit);
            var deposits = await Client.GetCurrentUserDeposits();

            deposits.Should().NotBeNull().And.HaveCount(1);
            deposits!.Count.Should().Be(TEST_TRANSACTION_COUNT);
            deposits[0].Amount.Should().Be(deposit.Amount);
            deposits[0].CurrencyId.Should().Be(currencyId);
            deposits[0].Status.Should().Be(DepositStatus.Undecided);
        }

        private static IEnumerable<TestCaseData> CreateDepositCorrectData()
        {
            yield return new TestCaseData(CRYPTO_CURRENCY_ID, CreateCryptoDepositDTO(null, null));
            yield return new TestCaseData(CRYPTO_CURRENCY_ID, CreateCryptoDepositDTO(FIAT_CARDHOLDER, FIAT_CARDNUMBER));
            yield return new TestCaseData(CRYPTO_CURRENCY_ID, CreateCryptoDepositDTO("123", "123"));
            yield return new TestCaseData(FIAT_CURRENCY_ID, CreateFiatDepositDTO(null));
            yield return new TestCaseData(FIAT_CURRENCY_ID, CreateFiatDepositDTO("123"));
            yield return new TestCaseData(FIAT_CURRENCY_ID, CreateFiatDepositDTO(CRYPTO_ADDRESS));
        }

        [TestCaseSource(nameof(CreateDepositIncorrectData))]
        public async Task CreateDeposit_WithIncorrectData_BadRequestAndDepositHasNotCreated(string currencyId, DepositDTO deposit)
        {
            await LoginAsUser(Client);

            var response = await Client.PutAsJsonAsync($"/wallet/deposit/{currencyId}", deposit);
            var deposits = await Client.GetCurrentUserDeposits();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            deposits.Should().NotBeNull().And.BeEmpty();
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

        private static DepositDTO CreateCryptoDepositDTO(string? cardholderName, string? cardNumber)
            => new(DEFAULT_DEPOSIT_AMOUNT, CRYPTO_ADDRESS, cardholderName, cardNumber);

        private static DepositDTO CreateFiatDepositDTO(string? address)
            => new(DEFAULT_DEPOSIT_AMOUNT, address, FIAT_CARDHOLDER, FIAT_CARDNUMBER);
        
        private async Task CreateUserClientAndCreateCryptoDeposit()
        {
            var userClient = Factory.CreateClient();
            await LoginAsUser(userClient);
            await CreateCryptoDeposit(userClient);
        }

        private async Task CreateAdminClientAndApproveTransaction(int id)
        {
            using var adminClient = Factory.CreateClient();
            await LoginAsAdmin(adminClient);
            await adminClient.ApproveTransaction(id);
        }

        private async static Task<HttpResponseMessage> CreateCryptoDeposit(HttpClient client)
        {
            var deposit = CreateCryptoDepositDTO(null, null);
            return await client.CreateDeposit(CRYPTO_CURRENCY_ID, deposit);
        }
    }
}

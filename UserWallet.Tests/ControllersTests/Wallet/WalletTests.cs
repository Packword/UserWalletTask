namespace UserWallet.Tests.ControllersTests.Wallet
{
    public class WalletTests: BaseControllerTest
    {
        private const int TEST_TRANSACTION_ID = 1;
        private const int TEST_TRANSACTION_COUNT = 1;

        [Test]
        public async Task GetCurrentUserBalance_AsUser_Success()
        {
            await LoginAsUser(_client);

            var response = await _client.GetAsync("wallet/balance");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task GetCurrentUserBalance_AsUserAfterDeposit_ReturnedBalanceIsCorrect()
        {
            await LoginAsUser(_client);
            await CreateCryptoDeposit(_client);
            await CreateAdminClientAndApproveTransaction(TEST_TRANSACTION_ID);

            var userBalance = await _client.GetCurrentUserBalance();

            userBalance.Should().NotBeNull().And.ContainKey(CRYPTO_CURRENCY_ID);
            userBalance![CRYPTO_CURRENCY_ID].Amount.Should().Be(DEFAULT_DEPOSIT_AMOUNT);
        }

        [Test]
        public async Task GetCurrentUserDeposits_AsUser_Success()
        {
            await LoginAsUser(_client);

            var response = await _client.GetAsync("wallet/tx");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task GetCurrentUserDeposits_AsUserWithOneCryptoDeposit_ReturnedDepositsCorrect()
        {
            await LoginAsUser(_client);
            await CreateCryptoDeposit(_client);

            var deposits = await _client.GetCurrentUserDeposits();

            deposits.Should().NotBeNull().And.HaveCount(TEST_TRANSACTION_COUNT);
            deposits![0].Amount.Should().Be(DEFAULT_DEPOSIT_AMOUNT);
            deposits[0].CurrencyId.Should().Be(CRYPTO_CURRENCY_ID);
            deposits[0].Status.Should().Be(DepositStatus.Undecided);
        }

        [Test]
        public async Task GetUserBalances_AsAdmin_Success()
        {
            await LoginAsAdmin(_client);

            var response = await _client.GetAsync($"wallet/{DEFAULT_USER_ID}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task GetUserBalances_AsAdminAfterDeposit_ReturnedBalancesIsCorrect()
        {
            await CreateUserClientAndCreateCryptoDeposit();
            await LoginAsAdmin(_client);
            await _client.ApproveTransaction(TEST_TRANSACTION_ID);

            var userBalance = await _client.GetUserBalance(DEFAULT_USER_ID);

            userBalance.Should().NotBeNull().And.ContainKey(CRYPTO_CURRENCY_ID);
            userBalance![CRYPTO_CURRENCY_ID].Amount.Should().Be(DEFAULT_DEPOSIT_AMOUNT);
        }

        

        [TestCaseSource(nameof(CreateDepositCorrectData))]
        public async Task CreateDeposit_AsUserWithCorrectData_Success(string currencyId, DepositDTO deposit)
        {
            await LoginAsUser(_client);

            var response = await _client.PutAsJsonAsync($"/wallet/deposit/{currencyId}", deposit);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestCaseSource(nameof(CreateDepositCorrectData))]
        public async Task CreateDeposit_AsUserWithCorrectData_DepositHasCreated(string currencyId, DepositDTO deposit)
        {
            await LoginAsUser(_client);


            await _client.PutAsJsonAsync($"/wallet/deposit/{currencyId}", deposit);
            var deposits = await _client.GetCurrentUserDeposits();

            deposits.Should().NotBeNull().And.HaveCount(1);
            deposits!.Count.Should().Be(TEST_TRANSACTION_COUNT);
            deposits[0].Amount.Should().Be(deposit.Amount);
            deposits[0].CurrencyId.Should().Be(currencyId);
            deposits[0].Status.Should().Be(DepositStatus.Undecided);
        }

        private static IEnumerable<TestCaseData> CreateDepositCorrectData()
        {
            yield return new TestCaseData(CRYPTO_CURRENCY_ID, CreateCryptoDepositDTOWithCorrectAddress(null, null));
            yield return new TestCaseData(CRYPTO_CURRENCY_ID, CreateCryptoDepositDTOWithCorrectAddress(FIAT_CARDHOLDER, FIAT_CARDNUMBER));
            yield return new TestCaseData(CRYPTO_CURRENCY_ID, CreateCryptoDepositDTOWithCorrectAddress("123", "123"));
            yield return new TestCaseData(FIAT_CURRENCY_ID, CreateFiatDepositDTOWithCorrectData(null));
            yield return new TestCaseData(FIAT_CURRENCY_ID, CreateFiatDepositDTOWithCorrectData("123"));
            yield return new TestCaseData(FIAT_CURRENCY_ID, CreateFiatDepositDTOWithCorrectData(CRYPTO_ADDRESS));
        }

        private static DepositDTO CreateCryptoDepositDTOWithCorrectAddress(string? cardholderName, string? cardNumber)
            => TransactionServiceHelper.CreateDepositDTO(DEFAULT_DEPOSIT_AMOUNT, CRYPTO_ADDRESS, cardholderName, cardNumber);

        private static DepositDTO CreateFiatDepositDTOWithCorrectData(string? address)
            => TransactionServiceHelper.CreateDepositDTO(DEFAULT_DEPOSIT_AMOUNT, address, FIAT_CARDHOLDER, FIAT_CARDNUMBER);
        
        private async Task CreateUserClientAndCreateCryptoDeposit()
        {
            var userClient = _factory.CreateClient();
            await LoginAsUser(userClient);
            await CreateCryptoDeposit(userClient);
        }

        private async Task CreateAdminClientAndApproveTransaction(int id)
        {
            using var adminClient = _factory.CreateClient();
            await LoginAsAdmin(adminClient);
            await adminClient.ApproveTransaction(id);
        }

        private async Task CreateCryptoDeposit(HttpClient client)
        {
            var deposit = CreateCryptoDepositDTOWithCorrectAddress(null, null);
            await client.CreateDeposit(CRYPTO_CURRENCY_ID, deposit);
        }
    }
}

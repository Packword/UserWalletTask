namespace UserWallet.Tests.ControllersTests.Wallet
{
    public class WalletTests: BaseControllerTest
    {
        private TransactionServiceHelper _transactionServiceHelper;
        private BalanceServiceHelper _balanceServiceHelper;
        private const int TEST_TRANSACTIONS_COUNT = 1;

        [SetUp]
        public new void Setup()
        {
            _transactionServiceHelper = new TransactionServiceHelper(_client);
            _balanceServiceHelper = new BalanceServiceHelper(_client);
        }

        [Test]
        public async Task GetCurrentUserBalance_AsUser_Success()
        {
            await _authServiceHelper.LoginAsUser();
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "wallet/balance");

            var response = await _client.SendAsync(requestMessage);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task GetCurrentUserBalance_AsUserAfterDeposit_ReturnedBalanceIsCorrect()
        {
            await _transactionServiceHelper.CreateFirstDepositAndApprove(TestData.CRYPTO_CURRENCY_ID, 
                                                                         TestData.DEFAULT_DEPOSIT_AMOUNT,
                                                                         TestData.CRYPTO_ADDRESS);
            await _authServiceHelper.LoginAsUser();

            var userBalance = await _balanceServiceHelper.GetCurrentUserBalance();

            userBalance![TestData.CRYPTO_CURRENCY_ID].Amount.Should().Be(TestData.DEFAULT_DEPOSIT_AMOUNT);
        }

        [Test]
        public async Task GetCurrentUserDeposits_AsUser_Success()
        {
            await _authServiceHelper.LoginAsUser();
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "wallet/tx");

            var response = await _client.SendAsync(requestMessage);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task GetCurrentUserDeposits_AsUserWithOneCryptoDeposit_ReturnedDepositsCorrect()
        {
            await _transactionServiceHelper.CreateCryptoDepositAsDefaultUser(TestData.CRYPTO_CURRENCY_ID,
                                                                TestData.DEFAULT_DEPOSIT_AMOUNT,
                                                                TestData.CRYPTO_ADDRESS);
            await _authServiceHelper.LoginAsUser();

            var deposits = await _transactionServiceHelper.GetCurrentUserDeposits();

            deposits!.Count.Should().Be(TEST_TRANSACTIONS_COUNT);
            deposits[0].Amount.Should().Be(TestData.DEFAULT_DEPOSIT_AMOUNT);
            deposits[0].CurrencyId.Should().Be(TestData.CRYPTO_CURRENCY_ID);
            deposits[0].Status.Should().Be(DepositStatus.Undecided);
        }

        [Test]
        public async Task GetUserBalances_AsAdmin_Success()
        {
            await _authServiceHelper.LoginAsAdmin();
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"wallet/{TestData.DEFAULT_USER_ID}");

            var response = await _client.SendAsync(requestMessage);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task GetUserBalances_AsAdminAfterDeposit_ReturnedBalancesIsCorrect()
        {
            await _transactionServiceHelper.CreateFirstDepositAndApprove(TestData.CRYPTO_CURRENCY_ID,
                                                                         TestData.DEFAULT_DEPOSIT_AMOUNT,
                                                                         TestData.CRYPTO_ADDRESS);
            await _authServiceHelper.LoginAsAdmin();

            var userBalance = await _balanceServiceHelper.GetUserBalance(TestData.DEFAULT_USER_ID);

            userBalance![TestData.CRYPTO_CURRENCY_ID].Amount.Should().Be(TestData.DEFAULT_DEPOSIT_AMOUNT);
        }

        [TestCaseSource(nameof(CreateDepositCorrectData))]
        public async Task CreateDeposit_AsUserWithCorrectData_Success(string currencyId, DepositDTO deposit)
        {
            await _authServiceHelper.LoginAsUser();
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"/wallet/deposit/{currencyId}");
            requestMessage.Content = JsonContent.Create(deposit);

            var response = await _client.SendAsync(requestMessage);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestCaseSource(nameof(CreateDepositCorrectData))]
        public async Task CreateDeposit_AsUserWithCorrectData_DepositHasCreated(string currencyId, DepositDTO deposit)
        {
            await _authServiceHelper.LoginAsUser();
            var requestMessageCreateDeposit = new HttpRequestMessage(HttpMethod.Put, $"/wallet/deposit/{currencyId}");
            requestMessageCreateDeposit.Content = JsonContent.Create(deposit);

            await _client.SendAsync(requestMessageCreateDeposit);
            var deposits = await _transactionServiceHelper.GetCurrentUserDeposits();

            deposits!.Count.Should().Be(TEST_TRANSACTIONS_COUNT);
            deposits[0].Amount.Should().Be(deposit.Amount);
            deposits[0].CurrencyId.Should().Be(currencyId);
            deposits[0].Status.Should().Be(DepositStatus.Undecided);
        }

        private static IEnumerable<TestCaseData> CreateDepositCorrectData()
        {
            yield return new TestCaseData(TestData.CRYPTO_CURRENCY_ID, CreateCryptoDepositDTOWithCorrectAddress(null, null));
            yield return new TestCaseData(TestData.CRYPTO_CURRENCY_ID, CreateCryptoDepositDTOWithCorrectAddress(TestData.FIAT_CARDHOLDER, TestData.FIAT_CARDNUMBER));
            yield return new TestCaseData(TestData.CRYPTO_CURRENCY_ID, CreateCryptoDepositDTOWithCorrectAddress("123", "123"));
            yield return new TestCaseData(TestData.FIAT_CURRENCY_ID, CreateFiatDepositDTOWithCorrectData(null));
            yield return new TestCaseData(TestData.FIAT_CURRENCY_ID, CreateFiatDepositDTOWithCorrectData("123"));
            yield return new TestCaseData(TestData.FIAT_CURRENCY_ID, CreateFiatDepositDTOWithCorrectData(TestData.CRYPTO_ADDRESS));
        }

        private static DepositDTO CreateDepositDTO(decimal amount, string? address, string? cardholderName, string? cardNumber)
            => new DepositDTO
            {
                Amount = amount,
                Address = address,
                CardholderName = cardholderName,
                CardNumber = cardNumber
            };

        private static DepositDTO CreateCryptoDepositDTOWithCorrectAddress(string? cardholderName, string? cardNumber)
            => CreateDepositDTO(TestData.DEFAULT_DEPOSIT_AMOUNT, TestData.CRYPTO_ADDRESS, cardholderName, cardNumber);

        private static DepositDTO CreateFiatDepositDTOWithCorrectData(string? address)
            => CreateDepositDTO(TestData.DEFAULT_DEPOSIT_AMOUNT, address, TestData.FIAT_CARDHOLDER, TestData.FIAT_CARDNUMBER);
    }
}

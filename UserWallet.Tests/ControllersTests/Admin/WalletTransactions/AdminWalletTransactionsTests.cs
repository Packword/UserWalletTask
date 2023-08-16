﻿namespace UserWallet.Tests.ControllersTests.Admin
{
    public class AdminWalletTransactionsTests: BaseControllerTest
    {
        private const int TEST_TRANSACTION_ID = 1;
        private const int TEST_TRANSACTION_COUNT = 1;

        [Test]
        public async Task GetTransactions_AsAdmin_Success()
        {
            await LoginAsAdmin(Client);    

            var response = await Client.GetAsync("admin/wallet/tx");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task GetTransactions_AsUser_Forbidden()
        {
            await LoginAsUser(Client);

            var response = await Client.GetAsync("admin/wallet/tx");

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Test]
        public async Task GetTransactions_AsAnonymous_Unauthorized()
        {
            var response = await Client.GetAsync("admin/wallet/tx");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
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

        [Test]
        public async Task ApproveTransaction_AsAdmin_Success()
        {
            await LoginAsAdmin(Client);
            await CreateUserClientAndCreateDeposit(CRYPTO_CURRENCY_ID, DEFAULT_DEPOSIT_AMOUNT, CRYPTO_ADDRESS);

            var response = await Client.ApproveTransaction(TEST_TRANSACTION_ID);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task ApproveTransaction_AsUser_Forbidden()
        {
            await LoginAsUser(Client);
            await CreateUserClientAndCreateDeposit(CRYPTO_CURRENCY_ID, DEFAULT_DEPOSIT_AMOUNT, CRYPTO_ADDRESS);

            var response = await Client.ApproveTransaction(TEST_TRANSACTION_ID);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Test]
        public async Task ApproveTransaction_AsAnonymous_Unauthorized()
        {
            await CreateUserClientAndCreateDeposit(CRYPTO_CURRENCY_ID, DEFAULT_DEPOSIT_AMOUNT, CRYPTO_ADDRESS);

            var response = await Client.ApproveTransaction(TEST_TRANSACTION_ID);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task ApproveTransaction_AsAdmin_TransactionHasBecomeApproved()
        {
            await LoginAsAdmin(Client);
            await CreateUserClientAndCreateDeposit(CRYPTO_CURRENCY_ID, DEFAULT_DEPOSIT_AMOUNT, CRYPTO_ADDRESS);

            await Client.ApproveTransaction(TEST_TRANSACTION_ID);
            var transactions = await Client.GetTransactions();

            transactions.Should().NotBeNull().And.HaveCount(TEST_TRANSACTION_COUNT);
            var transaction = transactions!.FirstOrDefault(u => u.Id == TEST_TRANSACTION_ID);
            transaction.Should().NotBeNull();
            transaction!.Status.Should().Be(DepositStatus.Approved);
        }

        [Test]
        public async Task ApproveTransaction_NonExistenTransaction_NotFound()
        {
            await LoginAsAdmin(Client);

            var response = await Client.ApproveTransaction(TEST_TRANSACTION_ID);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task ApproveTransaction_NotIntTxId_NotFound()
        {
            await LoginAsAdmin(Client);

            var response = await Client.PostAsJsonAsync($"admin/wallet/tx/approve/fasdg", "");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task ApproveTransaction_AsAdmin_BalanceHasChanged()
        {
            await LoginAsAdmin(Client);
            await CreateUserClientAndCreateDeposit(CRYPTO_CURRENCY_ID, DEFAULT_DEPOSIT_AMOUNT, CRYPTO_ADDRESS);

            await Client.ApproveTransaction(TEST_TRANSACTION_ID);
            var userBalances = await Client.GetUserBalance(DEFAULT_USER_ID);

            userBalances.Should().NotBeNull().And.ContainKey(CRYPTO_CURRENCY_ID);
            userBalances![CRYPTO_CURRENCY_ID].Amount.Should().Be(DEFAULT_DEPOSIT_AMOUNT);
        }

        [Test]
        public async Task ApproveTransaction_AlreadyApproved_BadRequestAndBalanceHasNotBeDoubled()
        {
            await LoginAsAdmin(Client);
            await CreateUserClientAndCreateDeposit(CRYPTO_CURRENCY_ID, DEFAULT_DEPOSIT_AMOUNT, CRYPTO_ADDRESS);
            await Client.ApproveTransaction(TEST_TRANSACTION_ID);

            var response = await Client.ApproveTransaction(TEST_TRANSACTION_ID);
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
            await Client.DeclineTransaction(TEST_TRANSACTION_ID);

            var response = await Client.ApproveTransaction(TEST_TRANSACTION_ID);
            var transactions = await Client.GetTransactions();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            transactions.Should().NotBeNull();
            var transaction = transactions!.FirstOrDefault(u => u.Id == TEST_TRANSACTION_ID);
            transaction.Should().NotBeNull();
            transaction!.Status.Should().Be(DepositStatus.Declined);
        }

        [Test]
        public async Task DeclineTransaction_AsAdmin_Success()
        {
            await LoginAsAdmin(Client);
            await CreateUserClientAndCreateDeposit(CRYPTO_CURRENCY_ID, DEFAULT_DEPOSIT_AMOUNT, CRYPTO_ADDRESS);

            var response = await Client.DeclineTransaction(TEST_TRANSACTION_ID);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task DeclineTransaction_AsUser_Forbidden()
        {
            await LoginAsUser(Client);
            await CreateUserClientAndCreateDeposit(CRYPTO_CURRENCY_ID, DEFAULT_DEPOSIT_AMOUNT, CRYPTO_ADDRESS);

            var response = await Client.DeclineTransaction(TEST_TRANSACTION_ID);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Test]
        public async Task DeclineTransaction_AsAnonymous_Unauthorized()
        {
            await CreateUserClientAndCreateDeposit(CRYPTO_CURRENCY_ID, DEFAULT_DEPOSIT_AMOUNT, CRYPTO_ADDRESS);

            var response = await Client.DeclineTransaction(TEST_TRANSACTION_ID);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task DeclineTransaction_NonExistenTransaction_NotFound()
        {
            await LoginAsAdmin(Client);

            var response = await Client.DeclineTransaction(TEST_TRANSACTION_ID);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task DeclineTransaction_NotIntTxId_NotFound()
        {
            await LoginAsAdmin(Client);

            var response = await Client.PostAsJsonAsync($"admin/wallet/tx/decline/fasdg", "");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task DeclineTransaction_AsAdmin_TransactionHasBecomeDeclined()
        {
            await LoginAsAdmin(Client);
            await CreateUserClientAndCreateDeposit(CRYPTO_CURRENCY_ID, DEFAULT_DEPOSIT_AMOUNT, CRYPTO_ADDRESS);

            await Client.DeclineTransaction(TEST_TRANSACTION_ID);
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

            await Client.DeclineTransaction(TEST_TRANSACTION_ID);
            var userBalances = await Client.GetUserBalance(DEFAULT_USER_ID);

            userBalances.Should().NotBeNull().And.NotContainKey(CRYPTO_CURRENCY_ID);
        }

        [Test]
        public async Task DeclineTransaction_AlreadyApproved_BadRequestAndStatusHasNotChanged()
        {
            await LoginAsAdmin(Client);
            await CreateUserClientAndCreateDeposit(CRYPTO_CURRENCY_ID, DEFAULT_DEPOSIT_AMOUNT, CRYPTO_ADDRESS);
            await Client.ApproveTransaction(TEST_TRANSACTION_ID);

            var response = await Client.DeclineTransaction(TEST_TRANSACTION_ID);
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
            await Client.DeclineTransaction(TEST_TRANSACTION_ID);

            var response = await Client.DeclineTransaction(TEST_TRANSACTION_ID);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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

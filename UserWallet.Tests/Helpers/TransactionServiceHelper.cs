namespace UserWallet.Tests.Helpers
{
    public class TransactionServiceHelper: BaseApiHelper
    {
        public TransactionServiceHelper(HttpClient client) : base(client)
        {
        }

        public async Task<HttpResponseMessage> CreateCryptoDepositAsDefaultUser(string currencyId, decimal amount, string address)
        {
            var authHelper = new AuthServiceHelper(_client);
            await authHelper.Logout();
            await authHelper.LoginAsUser();

            var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"/wallet/deposit/{currencyId}");
            requestMessage.Content = JsonContent.Create(CreateDepositDTO(amount, address, null, null));
            var response = await _client.SendAsync(requestMessage);

            await authHelper.Logout();
            return response;
        }

        public async Task<HttpResponseMessage> CreateFiatDepositAsDafaultUser(string currencyId, decimal amount, string cardholder, string cardnumber)
        {
            var authHelper = new AuthServiceHelper(_client);
            await authHelper.Logout();
            await authHelper.LoginAsUser();

            var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"/wallet/deposit/{currencyId}");
            requestMessage.Content = JsonContent.Create(CreateDepositDTO(amount, null, cardholder, cardnumber));
            var response = await _client.SendAsync(requestMessage);

            await authHelper.Logout();
            return response;
        }

        public async Task<List<Deposit>?> GetCurrentUserDeposits()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "wallet/tx");
            var response = await _client.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Deposit>>(content, TestData.JSON_OPTIONS);
        }

        public async Task<HttpResponseMessage> DeclineTransaction(int txId)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"admin/wallet/tx/decline/{txId}");
            return await _client.SendAsync(requestMessage);
        }

        public async Task<HttpResponseMessage> ApproveTransaction(int txId)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"admin/wallet/tx/approve/{txId}");
            return await _client.SendAsync(requestMessage);
        }

        public async Task CreateFirstDepositAndApprove(string currencyId, decimal amount, string address)
        {
            var authHelper = new AuthServiceHelper(_client);
            await CreateCryptoDepositAsDefaultUser(currencyId, amount, address);
            await authHelper.LoginAsAdmin();
            await ApproveTransaction(1);
            await authHelper.Logout();
        }

        public DepositDTO CreateDepositDTO(decimal amount, string? address, string? cardholderName, string? cardNumber)
            => new DepositDTO
            {
                Amount = amount,
                Address = address,
                CardholderName = cardholderName,
                CardNumber = cardNumber
            };
    }
}

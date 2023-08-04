namespace UserWallet.Tests.Helpers
{
    public class TransactionServiceHelper
    {
        
        private readonly HttpClient _client;
        public TransactionServiceHelper(HttpClient client)
        {
            _client = client;
        }
        public async Task<HttpResponseMessage> CreateCryptoDeposit(string currencyId, decimal amount, string address)
        {
            var authHelper = new AuthServiceHelper(_client);
            await authHelper.Logout();
            await authHelper.LoginAsUser();

            var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"/wallet/deposit/{currencyId}");
            requestMessage.Content = JsonContent.Create(new DepositDTO
            {
                Amount = amount,
                Address = address
            });
            var response = await _client.SendAsync(requestMessage);

            await authHelper.Logout();
            return response;
        }

        public async Task<HttpResponseMessage> CreateFiatDeposit(string currencyId, decimal amount, string cardholder, string cardnumber)
        {
            var authHelper = new AuthServiceHelper(_client);
            await authHelper.Logout();
            await authHelper.LoginAsUser();

            var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"/wallet/deposit/{currencyId}");
            requestMessage.Content = JsonContent.Create(new DepositDTO
            {
                Amount = amount,
                CardholderName = cardholder,
                CardNumber = cardnumber
            });
            var response = await _client.SendAsync(requestMessage);

            await authHelper.Logout();
            return response;
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

        public async Task CreateFirstDepositAndApprove(string currencyId, int amount, string address)
        {
            var authHelper = new AuthServiceHelper(_client);
            await CreateCryptoDeposit(currencyId, amount, address);
            await authHelper.LoginAsAdmin();
            await ApproveTransaction(1);
            await authHelper.Logout();
        }
    }
}

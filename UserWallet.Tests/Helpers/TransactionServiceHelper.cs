namespace UserWallet.Tests.Helpers
{
    public class TransactionServiceHelper
    {
        
        private readonly HttpClient _client;
        public TransactionServiceHelper(HttpClient client)
        {
            _client = client;
        }
        public async Task<HttpResponseMessage> CreateCryptoDeposit(string currencyId, int amount, string address)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"/wallet/deposit/{currencyId}");
            requestMessage.Content = JsonContent.Create(new DepositDTO
            {
                Amount = amount,
                Address = address
            });
            return await _client.SendAsync(requestMessage);
        }
    }
}

namespace UserWallet.Tests.Extensions
{
    public static class HttpClientBalanceExtension
    {
        public async static Task<Dictionary<string, BalanceDTO>?> GetCurrentUserBalance(this HttpClient client)
        {
            var response = await client.GetAsync("wallet/balance");
            return await response.GetContentAsync<Dictionary<string, BalanceDTO>>();
        }

        public async static Task<HttpResponseMessage> GetCurrentUserBalanceResponse(this HttpClient client)
            => await client.GetAsync("wallet/balance");

        public async static Task<Dictionary<string, BalanceDTO>?> GetUserBalance(this HttpClient client, int id)
        {
            var response = await client.GetAsync($"wallet/{id}");
            return await response.GetContentAsync<Dictionary<string, BalanceDTO>>();
        }

        public async static Task<HttpResponseMessage> GetUserBalanceResponse(this HttpClient client, string? id)
           => await client.GetAsync($"wallet/{id}");
    }
}

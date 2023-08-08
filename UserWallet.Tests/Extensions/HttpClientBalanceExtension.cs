namespace UserWallet.Tests.Extensions
{
    public static class HttpClientBalanceExtension
    {
        public async static Task<Dictionary<string, BalanceDTO>?> GetCurrentUserBalance(this HttpClient client)
        {
            var response = await client.GetAsync("wallet/balance");
            return await response.Content.ReadFromJsonAsync<Dictionary<string, BalanceDTO>>(TestOptions.JSON_OPTIONS);
        }

        public async static Task<Dictionary<string, BalanceDTO>?> GetUserBalance(this HttpClient client, int id)
        {
            var response = await client.GetAsync($"wallet/{id}");
            return await response.Content.ReadFromJsonAsync<Dictionary<string, BalanceDTO>>(TestOptions.JSON_OPTIONS);
        }
    }
}

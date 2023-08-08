namespace UserWallet.Tests.Extensions
{
    public static class HttpClientTransactionsExtension
    {
        public async static Task<HttpResponseMessage> CreateDeposit(this HttpClient client, string currencyId, DepositDTO deposit)
            => await client.PutAsJsonAsync($"/wallet/deposit/{currencyId}", JsonSerializer.Serialize(deposit));

        public async static Task<List<Deposit>?> GetCurrentUserDeposits(this HttpClient client)
        {
            var response = await client.GetAsync("wsllet/tx");
            return await response.Content.ReadFromJsonAsync<List<Deposit>>(TestOptions.JSON_OPTIONS);
        }

        public async static Task<List<Deposit>?> GetTransactions(this HttpClient client)
        {
            var response = await client.GetAsync("admin/wallet/tx");
            return await response.Content.ReadFromJsonAsync<List<Deposit>>(TestOptions.JSON_OPTIONS);
        }

        public async static Task<HttpResponseMessage> DeclineTransaction(this HttpClient client, int txId)
            => await client.PostAsJsonAsync($"admin/wallet/tx/decline/{txId}", "");

        public async static Task<HttpResponseMessage> ApproveTransaction(this HttpClient client, int txId)
            => await client.PostAsJsonAsync($"admin/wallet/tx/approve/{txId}", "");
    }
}

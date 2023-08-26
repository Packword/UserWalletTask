namespace UserWallet.Tests.Extensions
{
    public static class HttpClientTransactionsExtension
    {
        public async static Task<HttpResponseMessage> CreateDeposit(this HttpClient client, string currencyId, DepositDTO deposit)
            => await client.PutAsJsonAsync($"/wallet/deposit/{currencyId}", deposit);

        public async static Task<List<Deposit>?> GetCurrentUserDeposits(this HttpClient client)
        {
            var response = await client.GetAsync("wallet/tx");
            return await response.GetContentAsync<List<Deposit>>();
        }

        public async static Task<HttpResponseMessage> GetCurrentUserDepositsResponse(this HttpClient client)
            => await client.GetAsync("wallet/tx");
    }
}

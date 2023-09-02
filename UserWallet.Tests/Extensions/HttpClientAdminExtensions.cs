namespace UserWallet.Tests.Extensions
{
    public static class HttpClientAdminExtensions
    {
        public async static Task<HttpResponseMessage> GetUsers(this HttpClient client)
            => await client.GetAsync("admin/users");

        public async static Task<HttpResponseMessage> BlockUser(this HttpClient client, string? userId)
            => await client.PatchAsJsonAsync($"admin/users/block/{userId}", "");

        public async static Task<HttpResponseMessage> UnblockUser(this HttpClient client, string? userId)
            => await client.PatchAsJsonAsync($"admin/users/unblock/{userId}", "");

        public async static Task<List<Deposit>?> GetTransactions(this HttpClient client)
        {
            var response = await client.GetAsync("admin/wallet/tx");
            return await response.GetContentAsync<List<Deposit>>();
        }

        public async static Task<HttpResponseMessage> GetTransactionsResponse(this HttpClient client)
            => await client.GetAsync("admin/wallet/tx");

        public async static Task<HttpResponseMessage> DeclineTransaction(this HttpClient client, string? txId)
            => await client.PostAsJsonAsync($"admin/wallet/tx/decline/{txId}", "");

        public async static Task<HttpResponseMessage> ApproveTransaction(this HttpClient client, string? txId)
            => await client.PostAsJsonAsync($"admin/wallet/tx/approve/{txId}", "");
    }
}

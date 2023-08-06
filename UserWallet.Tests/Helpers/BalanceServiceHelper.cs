namespace UserWallet.Tests.Helpers
{
    public class BalanceServiceHelper : BaseApiHelper
    {
        public BalanceServiceHelper(HttpClient client) : base(client)
        {
        }

        public async Task<Dictionary<string, BalanceDTO>?> GetCurrentUserBalance()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "wallet/balance");
            var response = await _client.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Dictionary<string, BalanceDTO>>(content, TestData.JSON_OPTIONS);
        }

        public async Task<Dictionary<string, BalanceDTO>?> GetUserBalance(int id)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"wallet/{id}");
            var response = await _client.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Dictionary<string, BalanceDTO>>(content, TestData.JSON_OPTIONS);
        }
    }
}

namespace UserWallet.Tests.Helpers
{
    public class AuthServiceHelper
    {
        private readonly HttpClient _client;
        public AuthServiceHelper(HttpClient client)
        {
            _client = client;
        }

        public async Task<HttpResponseMessage> Logout()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/auth/logout");
            var response = await _client.SendAsync(requestMessage);
            return response;
        }


        public async Task<HttpResponseMessage> Login(string username, string password)
        {
            HttpRequestMessage requestMessage = GenerateLoginRequestMessage(username, password);
            return await _client.SendAsync(requestMessage);
        }

        public async Task<HttpResponseMessage> LoginAsAdmin()
        {
            HttpRequestMessage requestMessage = GenerateLoginRequestMessage(TestData.ADMIN_USERNAME, TestData.ADMIN_PASSWORD);
            return await _client.SendAsync(requestMessage);
        }

        public async Task<HttpResponseMessage> LoginAsUser()
        {
            HttpRequestMessage requestMessage = GenerateLoginRequestMessage(TestData.DEFAULT_USER_USERNAME, TestData.DEFAULT_USER_PASSWORD);
            return await _client.SendAsync(requestMessage);
        }

        private static HttpRequestMessage GenerateLoginRequestMessage(string username, string password)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/auth/login");
            requestMessage.Content = JsonContent.Create(new LoginDTO
            {
                Username = username,
                Password = password,
            });
            return requestMessage;
        }
    }
}

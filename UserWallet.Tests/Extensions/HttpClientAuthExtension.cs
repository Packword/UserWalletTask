namespace UserWallet.Tests.Extensions
{
    public static class HttpClientAuthExtension
    {
        public async static Task<HttpResponseMessage> Login(this HttpClient client, string username, string password)
            => await client.PostAsJsonAsync(
                "/auth/login",
                CreateLoginDTO(username, password));

        public async static Task<HttpResponseMessage> Logout(this HttpClient client)
            => await client.PostAsJsonAsync("/auth/logout", "");

        private static LoginDTO CreateLoginDTO(string username, string password)
            => new LoginDTO
            {
                Username = username,
                Password = password
            };
    }
}

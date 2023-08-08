namespace UserWallet.Tests.Extensions
{
    public static class HttpClientAuthExtension
    {
        public async static Task<HttpResponseMessage> Login(this HttpClient client, string username, string password)
            => await client.PostAsJsonAsync(
                "http://localhost:5140/auth/login",
                CreateSerializedLoginDTO(username, password));

        public async static Task<HttpResponseMessage> Logout(this HttpClient client)
            => await client.PostAsJsonAsync("/auth/logout", "");

        private static string CreateSerializedLoginDTO(string username, string password)
        {
            return JsonSerializer.Serialize(new LoginDTO
            {
                Username = username,
                Password = password
            });
        }
    }
}

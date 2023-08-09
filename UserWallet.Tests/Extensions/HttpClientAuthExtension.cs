namespace UserWallet.Tests.Extensions
{
    public static class HttpClientAuthExtension
    {
        public async static Task<HttpResponseMessage> Login(this HttpClient client, string? username, string? password)
            => await client.PostAsJsonAsync(
                "/auth/login",
                CreateLoginDTO(username, password));

        public async static Task<HttpResponseMessage> Logout(this HttpClient client)
            => await client.PostAsJsonAsync("/auth/logout", "");

        public async static Task<HttpResponseMessage> SignUp(this HttpClient client, string? username, string? password)
            => await client.PostAsJsonAsync(
                "/auth/sign-up",
                CreateSignUpDTO(username, password));

        private static SignUpDTO CreateSignUpDTO(string? username, string? password)
            => new()
            {
                Username = username,
                Password = password
            };

        private static LoginDTO CreateLoginDTO(string? username, string? password)
            => new()
            {
                Username = username,
                Password = password
            };
    }
}

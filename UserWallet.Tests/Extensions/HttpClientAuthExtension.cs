﻿namespace UserWallet.Tests.Extensions
{
    public static class HttpClientAuthExtension
    {
        public async static Task<HttpResponseMessage> Login(this HttpClient client, string? username, string? password)
            => await client.PostAsJsonAsync(
                "/auth/login",
                new LoginDTO(username, password));

        public async static Task<HttpResponseMessage> Logout(this HttpClient client)
            => await client.PostAsJsonAsync("/auth/logout", "");

        public async static Task<HttpResponseMessage> SignUp(this HttpClient client, string? username, string? password)
            => await client.PostAsJsonAsync(
                "/auth/sign-up",
                new SignUpDTO(username, password));

        public async static Task<HttpResponseMessage> ChangePassword(this HttpClient client, string? newPassword, string? oldPassword)
            => await client.PatchAsJsonAsync(
                "/auth/change-password",
                new ChangeUserPasswordDTO { NewPassword = newPassword, OldPassword = oldPassword });
    }
}

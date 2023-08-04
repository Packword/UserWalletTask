namespace UserWallet.Tests.ControllersTests.Auth
{
    public class AuthPositiveTests : BaseControllerTest
    {
        [Test]
        public async Task PositiveLoginAsAdminTest()
        {
            var response = await _authServiceHelper.LoginAsync("Admin", "1234");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task PositiveLoginAsUserTest()
        {
            var response = await _authServiceHelper.LoginAsync("maxim", "123456");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task PositiveChangePasswordTest()
        {
            await _authServiceHelper.LoginAsync("Admin", "1234");
            var requestMessage = new HttpRequestMessage(HttpMethod.Patch, "/auth/change-password");
            requestMessage.Content = JsonContent.Create(new ChangeUserPasswordDTO { NewPassword = "12345" });
            var response = await _client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            await _authServiceHelper.Logout();
            response = await _authServiceHelper.LoginAsync("Admin", "12345");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

        }

        [Test]
        public async Task PositiveLogoutTest()
        {
            await _authServiceHelper.LoginAsync("Admin", "1234");
            HttpResponseMessage response = await _authServiceHelper.Logout();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task PositiveSignInTest()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/auth/sign-up");
            requestMessage.Content = JsonContent.Create(new SignUpDTO
            {
                Username = "ForTest",
                Password = "Test"
            });
            var response = await _client.SendAsync(requestMessage);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response = await _authServiceHelper.LoginAsync("ForTest", "Test");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
namespace UserWallet.Tests.ControllersTests.Auth
{
    public class Auth : BaseControllerTest
    {
        [Test]
        public async Task Login_AsAdmin_Success()
        {
            var response = await _authServiceHelper.LoginAsAdmin();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Login_AsUser_Success()
        {
            var response = await _authServiceHelper.LoginAsUser();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task ChangePassword_AsAdmin_Success()
        {
            await _authServiceHelper.LoginAsAdmin();
            var requestMessage = new HttpRequestMessage(HttpMethod.Patch, "/auth/change-password");
            requestMessage.Content = JsonContent.Create(new ChangeUserPasswordDTO { NewPassword = "12345" });

            var response = await _client.SendAsync(requestMessage);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task ChangePassword_AsAdmin_SuccessLoginWithNewPassword()
        {
            await _authServiceHelper.LoginAsAdmin();
            var requestMessage = new HttpRequestMessage(HttpMethod.Patch, "/auth/change-password");
            requestMessage.Content = JsonContent.Create(new ChangeUserPasswordDTO { NewPassword = "12345" });
            await _client.SendAsync(requestMessage);
            await _authServiceHelper.Logout();

            var response = await _authServiceHelper.Login(TestData.ADMIN_USERNAME, "12345");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Logout_FromAdmin_Success()
        {
            await _authServiceHelper.LoginAsAdmin();

            var response = await _authServiceHelper.Logout();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task SignUp_CorrectUser_Success()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/auth/sign-up");
            var testUser = _authServiceHelper.CreateSignUpDTO("ForTest", "Test");
            requestMessage.Content = JsonContent.Create(testUser);

            var response = await _client.SendAsync(requestMessage);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task SignUp_CorrectUser_SuccessLoginAsNewUser()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/auth/sign-up");
            var testUser = _authServiceHelper.CreateSignUpDTO("ForTest", "Test");
            requestMessage.Content = JsonContent.Create(testUser);
            await _client.SendAsync(requestMessage);

            var response = await _authServiceHelper.Login("ForTest", "Test");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
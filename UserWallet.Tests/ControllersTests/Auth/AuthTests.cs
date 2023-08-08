namespace UserWallet.Tests.ControllersTests.Auth
{
    public class AuthTests : BaseControllerTest
    {
        [Test]
        public async Task Login_AsAdmin_Success()
        {
            var response = await LoginAsAdmin(_client);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Login_AsUser_Success()
        {
            var response = await LoginAsUser(_client);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task ChangePassword_AsAdmin_Success()
        {
            await LoginAsAdmin(_client);

            var response = await _client.PatchAsJsonAsync(
                "/auth/change-password", 
                JsonSerializer.Serialize(new ChangeUserPasswordDTO { NewPassword = "12345" })
            );

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task ChangePassword_AsAdmin_SuccessLoginWithNewPassword()
        {
            await LoginAsAdmin(_client);
            await _client.PatchAsJsonAsync(
                "/auth/change-password",
                JsonSerializer.Serialize(new ChangeUserPasswordDTO { NewPassword = "12345" })
            );
            await _client.Logout();

            var response = await _client.Login(ADMIN_USERNAME, "12345");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Logout_FromAdmin_Success()
        {
            await LoginAsAdmin(_client);

            var response = await _client.Logout();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task SignUp_CorrectUser_Success()
        {
            var testUser = CreateSignUpDTO("ForTest", "Test");

            var response = await _client.PostAsJsonAsync("/auth/sign-up", JsonSerializer.Serialize(testUser));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task SignUp_CorrectUser_SuccessLoginAsNewUser()
        {
            var testUser = CreateSignUpDTO("ForTest", "Test");
            await _client.PostAsJsonAsync("/auth/sign-up", JsonSerializer.Serialize(testUser));

            var response = await _client.Login("ForTest", "Test");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        private SignUpDTO CreateSignUpDTO(string username, string password)
        {
            return new SignUpDTO
            {
                Username = username,
                Password = password
            };
        }
    }
}
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
        public async Task Login_ReLoginWithoutLogout_BadRequset()
        {
            await LoginAsUser(_client);

            var response = await LoginAsAdmin(_client);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Login_NonExistentUser_Unauthorized()
        {
            var response = await _client.Login("Test", "Test");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task Login_WithIncorrectData_BadRequest()
        {
            var response = await _client.Login(null, null);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task ChangePassword_AsAdmin_Success()
        {
            await LoginAsAdmin(_client);

            var response = await _client.PatchAsJsonAsync(
                "/auth/change-password", 
                new ChangeUserPasswordDTO { NewPassword = "12345" }
            );

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestCaseSource(nameof(ChangePasswordIncorrectData))]
        public async Task ChangePassword_AsUserWithIncorrectData_BadRequest(ChangeUserPasswordDTO? passwordDto)
        {
            await LoginAsUser(_client);

            var response = await _client.PatchAsJsonAsync(
                "/auth/change-password",
                passwordDto
            );

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        private static IEnumerable<TestCaseData> ChangePasswordIncorrectData()
        {
            yield return new TestCaseData( new ChangeUserPasswordDTO { NewPassword = "123" });
            yield return new TestCaseData( new ChangeUserPasswordDTO { NewPassword = "12345678901234567890" });
            yield return new TestCaseData( new ChangeUserPasswordDTO { NewPassword = null });
            yield return new TestCaseData( new ChangeUserPasswordDTO());
            yield return new TestCaseData( null ); 
        }

        [Test]
        public async Task ChangePassword_AsAdmin_SuccessLoginWithNewPassword()
        {
            await LoginAsAdmin(_client);
            await _client.PatchAsJsonAsync(
                "/auth/change-password",
                new ChangeUserPasswordDTO { NewPassword = "12345" }
            );
            await _client.Logout();

            var response = await _client.Login(ADMIN_USERNAME, "12345");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task ChangePassword_AsAnonymous_Unauthorized()
        {
            var response = await _client.PatchAsJsonAsync(
                "/auth/change-password",
                new ChangeUserPasswordDTO { NewPassword = "12345" }
            );

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task Logout_FromAdmin_Success()
        {
            await LoginAsAdmin(_client);

            var response = await _client.Logout();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Logout_AsAnonymous_Unauthorized()
        {
            var response = await _client.Logout();

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task SignUp_CorrectUser_Success()
        {
            var response = await _client.SignUp("ForTest", "Test");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task SignUp_ExistingUser_BadRequest()
        {
            var response = await _client.SignUp(ADMIN_USERNAME, "Test");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [TestCaseSource(nameof(SignUpIncorrectData))]
        public async Task SignUp_IncorrectUser_BadRequest(string? username, string password)
        {
            var response = await _client.SignUp(username, password);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        private static IEnumerable<TestCaseData> SignUpIncorrectData()
        {
            yield return new TestCaseData(null, null);
            yield return new TestCaseData("12345", null);
            yield return new TestCaseData(null, "12345");
            yield return new TestCaseData("", "");
            yield return new TestCaseData("123", "12345");
            yield return new TestCaseData("12345", "123");
            yield return new TestCaseData("12345678901234567890", "12345");
        }

        [Test]
        public async Task SignUp_CorrectUser_SuccessLoginAsNewUser()
        {
            await _client.SignUp("ForTest", "Test");

            var response = await _client.Login("ForTest", "Test");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
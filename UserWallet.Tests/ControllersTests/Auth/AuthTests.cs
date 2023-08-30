namespace UserWallet.Tests.ControllersTests.Auth
{
    public class AuthTests : BaseControllerTest
    {
        private static (string Username, string Password) Admin = new(ADMIN_USERNAME, ADMIN_PASSWORD);
        private static (string Username, string Password) DefaultUser = new(DEFAULT_USER_USERNAME, DEFAULT_USER_PASSWORD);
        private const string TEST_USERNAME = "ForTest";
        private const string TEST_PASSWORD = "Test";

        [TestCaseSource(nameof(AccessLoginData))]
        public async Task<HttpStatusCode> Login_DifferentData_CorrectResponse((string Username, string Password)? user)
        {
            var response = await Login(user);

            return response.StatusCode;
        }

        [Test]
        public async Task Login_ReLoginWithoutLogout_BadRequest()
        {
            await LoginAsUser(Client);

            var response = await LoginAsAdmin(Client);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [TestCaseSource(nameof(AccessChangePasswordData))]
        public async Task<HttpStatusCode> ChangePassword_CorrectData_CorrectResponse((string Username, string Password)? user, string oldPassword)
        {
            await Login(user);

            var response = await Client.ChangePassword(TEST_PASSWORD, oldPassword);

            return response.StatusCode;
        }

        [TestCaseSource(nameof(ChangePasswordIncorrectData))]
        public async Task ChangePassword_IncorrectData_BadRequest(ChangeUserPasswordDTO? passwordDto, Dictionary<string, string>? errorList)
        {
            await LoginAsUser(Client);

            var response = await Client.ChangePassword(passwordDto?.NewPassword, passwordDto?.OldPassword);

            if (errorList is not null)
            {
                var errors = response.GetErrors();
                errors.Should().NotBeNull().And.HaveCountGreaterThan(0);
                foreach (var error in errorList)
                {
                    errors.Should().ContainKey(error.Key);
                    errors![error.Key].Should().Contain(error.Value);
                }
            }
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task ChangePassword_AsAdmin_SuccessLoginWithNewPassword()
        {
            await LoginAsAdmin(Client);
            await Client.ChangePassword(TEST_PASSWORD, ADMIN_PASSWORD);
            await Client.Logout();

            var response = await Client.Login(ADMIN_USERNAME, TEST_PASSWORD);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestCaseSource(nameof(AccessLogoutData))]
        public async Task<HttpStatusCode> Logout_DifferentUsers_CorrectResponse((string Username, string Password)? user)
        {
            await Login(user);

            var response = await Client.Logout();

            return response.StatusCode;
        }

        [TestCaseSource(nameof(SignUpData))]
        public async Task<HttpStatusCode> SignUp_DifferentUsers_CorrectAnswer(string? username, string password)
        {
            var response = await Client.SignUp(username, password);

            return response.StatusCode;
        }

        [Test]
        public async Task SignUp_CorrectUser_SuccessLoginAsNewUser()
        {
            var responseSignUp = await SignUpAsTestUser();
            var responseLogin = await Client.Login(TEST_USERNAME, TEST_PASSWORD);

            responseSignUp.StatusCode.Should().Be(HttpStatusCode.OK);
            responseLogin.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        private static IEnumerable<TestCaseData> ChangePasswordIncorrectData()
        {
            yield return new TestCaseData(new ChangeUserPasswordDTO { NewPassword = "123", OldPassword = ADMIN_PASSWORD },
                                           new Dictionary<string, string>() {
                                               {"NewPassword", "The field NewPassword must be a string with a minimum length of 4 and a maximum length of 8."}
                                           });
            yield return new TestCaseData(new ChangeUserPasswordDTO { NewPassword = "12345", OldPassword = "1234567" },
                                           new Dictionary<string, string>() {
                                               {"default", "Wrong old password"}
                                           });
            yield return new TestCaseData(new ChangeUserPasswordDTO { NewPassword = "12345678901234567890", OldPassword = ADMIN_PASSWORD },
                                           new Dictionary<string, string>() {
                                               {"NewPassword", "The field NewPassword must be a string with a minimum length of 4 and a maximum length of 8."}
                                           });
            yield return new TestCaseData(new ChangeUserPasswordDTO { NewPassword = null, OldPassword = ADMIN_PASSWORD },
                                           new Dictionary<string, string>() {
                                               {"NewPassword", "The NewPassword field is required."}
                                           });
            yield return new TestCaseData(null, new Dictionary<string, string>() {
                                               {"NewPassword", "The NewPassword field is required."},
                                               {"OldPassword", "The OldPassword field is required."}
                                           });
        }

        private static IEnumerable<TestCaseData> SignUpData()
        {
            yield return new TestCaseData(null, null).Returns(HttpStatusCode.BadRequest);
            yield return new TestCaseData("12345", null).Returns(HttpStatusCode.BadRequest);
            yield return new TestCaseData(null, "12345").Returns(HttpStatusCode.BadRequest);
            yield return new TestCaseData("", "").Returns(HttpStatusCode.BadRequest);
            yield return new TestCaseData("123", "12345").Returns(HttpStatusCode.BadRequest);
            yield return new TestCaseData("12345", "123").Returns(HttpStatusCode.BadRequest);
            yield return new TestCaseData("12345678901234567890", "12345").Returns(HttpStatusCode.BadRequest);
            yield return new TestCaseData(ADMIN_USERNAME, TEST_PASSWORD).Returns(HttpStatusCode.BadRequest);
            yield return new TestCaseData(TEST_USERNAME, TEST_PASSWORD).Returns(HttpStatusCode.OK);
        }

        private static IEnumerable<TestCaseData> AccessLoginData()
        {
            yield return new TestCaseData(Admin).Returns(HttpStatusCode.OK);
            yield return new TestCaseData(DefaultUser).Returns(HttpStatusCode.OK);
            yield return new TestCaseData(("Test", "Test")).Returns(HttpStatusCode.Unauthorized);
            yield return new TestCaseData(null).Returns(HttpStatusCode.BadRequest);
        }
        private static IEnumerable<TestCaseData> AccessChangePasswordData()
        {
            yield return new TestCaseData(Admin, ADMIN_PASSWORD).Returns(HttpStatusCode.OK);
            yield return new TestCaseData(DefaultUser, DEFAULT_USER_PASSWORD).Returns(HttpStatusCode.OK);
            yield return new TestCaseData(("Test", "Test"), "1234").Returns(HttpStatusCode.Unauthorized);
        }

        private static IEnumerable<TestCaseData> AccessLogoutData()
        {
            yield return new TestCaseData(Admin).Returns(HttpStatusCode.OK);
            yield return new TestCaseData(DefaultUser).Returns(HttpStatusCode.OK);
            yield return new TestCaseData(("Test", "Test")).Returns(HttpStatusCode.Unauthorized);
        }

        private async Task<HttpResponseMessage> SignUpAsTestUser()
            => await Client.SignUp(TEST_USERNAME, TEST_PASSWORD);

        private async Task<HttpResponseMessage> Login((string Username, string Password)? user)
            => await Client.Login(user?.Username, user?.Password);
    }
}
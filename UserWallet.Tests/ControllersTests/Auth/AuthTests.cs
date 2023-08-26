using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace UserWallet.Tests.ControllersTests.Auth
{
    public class AuthTests : BaseControllerTest
    {
        private static (string Username, string Password) Admin = new(ADMIN_USERNAME, ADMIN_PASSWORD);
        private static (string Username, string Password) DefaultUser = new(DEFAULT_USER_USERNAME, DEFAULT_USER_PASSWORD);

        [TestCaseSource(nameof(AccessLoginTestData))]
        public async Task Login_DifferentData_CorrectResponse((string Username, string Password)? user, HttpStatusCode exceptedResponse)
        {
            var response = await Login(user);

            response.StatusCode.Should().Be(exceptedResponse);
        }

        [Test]
        public async Task Login_ReLoginWithoutLogout_BadRequest()
        {
            await LoginAsUser(Client);

            var response = await LoginAsAdmin(Client);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [TestCaseSource(nameof(AccessChangePasswordTestData))]
        public async Task ChangePassword_CorrectData_CorrectResponse((string Username, string Password)? user, HttpStatusCode exceptedResponse, string oldPassword)
        {
            await Login(user);

            var response = await Client.PatchAsJsonAsync(
                "/auth/change-password", 
                new ChangeUserPasswordDTO { NewPassword = "12345", OldPassword = oldPassword }
            );

            response.StatusCode.Should().Be(exceptedResponse);
        }

        [TestCaseSource(nameof(ChangePasswordIncorrectData))]
        public async Task ChangePassword_IncorrectData_BadRequest(ChangeUserPasswordDTO? passwordDto, Dictionary<string, string>? errorList)
        {
            await LoginAsUser(Client);

            var response = await Client.PatchAsJsonAsync(
                "/auth/change-password",
                passwordDto
            );

            if (errorList is not null)
            {
                var errors = response.GetErrors();
                errors.Should().NotBeNull().And.HaveCountGreaterThan(0);
                foreach (var error in errorList)
                {
                    errors.Should().ContainKey(error.Key);
                    errors[error.Key].Should().Contain(error.Value);
                }
            }
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        private static IEnumerable<TestCaseData> ChangePasswordIncorrectData()
        {
            yield return new TestCaseData( new ChangeUserPasswordDTO { NewPassword = "123", OldPassword = ADMIN_PASSWORD },
                                           new Dictionary<string, string>() {
                                               {"NewPassword", "The field NewPassword must be a string with a minimum length of 4 and a maximum length of 8."}
                                           });
            yield return new TestCaseData( new ChangeUserPasswordDTO { NewPassword = "12345", OldPassword = "1234567" },
                                           new Dictionary<string, string>() {
                                               {"default", "Wrong old password"}
                                           });
            yield return new TestCaseData( new ChangeUserPasswordDTO { NewPassword = "12345678901234567890", OldPassword = ADMIN_PASSWORD },
                                           new Dictionary<string, string>() {
                                               {"NewPassword", "The field NewPassword must be a string with a minimum length of 4 and a maximum length of 8."}
                                           });
            yield return new TestCaseData( new ChangeUserPasswordDTO { NewPassword = null, OldPassword = ADMIN_PASSWORD },
                                           new Dictionary<string, string>() {
                                               {"NewPassword", "The NewPassword field is required."}
                                           });
            yield return new TestCaseData(null, new Dictionary<string, string>() {
                                               {"", "A non-empty request body is required."}
                                           }); 
        }

        [Test]
        public async Task ChangePassword_AsAdmin_SuccessLoginWithNewPassword()
        {
            const string  NEW_PASSWORD = "12345";

            await LoginAsAdmin(Client);
            await Client.PatchAsJsonAsync(
                "/auth/change-password",
                new ChangeUserPasswordDTO { NewPassword = NEW_PASSWORD, OldPassword = ADMIN_PASSWORD }
            );
            await Client.Logout();

            var response = await Client.Login(ADMIN_USERNAME, NEW_PASSWORD);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestCaseSource(nameof(AccessLogoutTestData))]
        public async Task Logout_DifferentUsers_CorrectResponse((string Username, string Password)? user, HttpStatusCode exceptedResponse)
        {
            await Login(user);

            var response = await Client.Logout();

            response.StatusCode.Should().Be(exceptedResponse);
        }

        [Test]
        public async Task SignUp_CorrectUser_Success()
        {
            var response = await Client.SignUp("ForTest", "Test");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task SignUp_ExistingUser_BadRequest()
        {
            var response = await Client.SignUp(ADMIN_USERNAME, "Test");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [TestCaseSource(nameof(SignUpIncorrectData))]
        public async Task SignUp_IncorrectUser_BadRequest(string? username, string password)
        {
            var response = await Client.SignUp(username, password);

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

        private static IEnumerable<TestCaseData> AccessLoginTestData()
        {
            yield return new TestCaseData(Admin, HttpStatusCode.OK);
            yield return new TestCaseData(DefaultUser, HttpStatusCode.OK);
            yield return new TestCaseData(("Test", "Test"), HttpStatusCode.Unauthorized);
            yield return new TestCaseData(null, HttpStatusCode.BadRequest);
        }
        private static IEnumerable<TestCaseData> AccessChangePasswordTestData()
        {
            yield return new TestCaseData(Admin, HttpStatusCode.OK, ADMIN_PASSWORD);
            yield return new TestCaseData(DefaultUser, HttpStatusCode.OK, DEFAULT_USER_PASSWORD);
            yield return new TestCaseData(("Test", "Test"), HttpStatusCode.Unauthorized, "1234");
        }

        private static IEnumerable<TestCaseData> AccessLogoutTestData()
        {
            yield return new TestCaseData(Admin, HttpStatusCode.OK);
            yield return new TestCaseData(DefaultUser, HttpStatusCode.OK);
            yield return new TestCaseData(("Test", "Test"), HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task SignUp_CorrectUser_SuccessLoginAsNewUser()
        {
            const string USERNAME = "ForTest";
            const string PASSWORD = "Test";

            await Client.SignUp(USERNAME, PASSWORD);

            var response = await Client.Login(USERNAME, PASSWORD);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        private async Task<HttpResponseMessage> Login((string Username, string Password)? user)
            => await Client.Login(user?.Username, user?.Password);
    }
}
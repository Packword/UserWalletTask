using UserWallet.Models;

namespace UserWallet.Tests.ControllersTests.Admin
{
    public class AdminUsersTests: BaseControllerTest
    {
        private const int NON_EXISTENT_USER_ID = 999;

        [TestCaseSource(nameof(AccessGetUsersData))]
        public async Task<HttpStatusCode> GetUsers_DifferentUsers_CorrectAnswer(string? username, string? password)
        {
            await Client.Login(username, password);

            var response = await Client.GetUsers();

            return response.StatusCode;
        }

        [Test]
        public async Task GetUsers_AsAdmin_ReturnsCorrectUsersCount()
        {
            await LoginAsAdmin(Client);
            
            var users = await GetUsers();

            users.Should().NotBeNull();
            users!.Should().HaveCount(DEFAULT_USERS_COUNT);
        }

        [TestCaseSource(nameof(BlockAndUnblockUserData))]
        public async Task<HttpStatusCode> Block_DifferentUsers_CorrectAnswer(string? username, string? password, string? userId)
        {
            await Client.Login(username, password);

            var response = await Client.BlockUser(userId);

            return response.StatusCode;
        }

        [TestCaseSource(nameof(BlockAndUnblockUserData))]
        public async Task<HttpStatusCode> Unblock_DifferentUsers_CorrectAnswer(string? username, string? password, string? userId)
        {
            await Client.Login(username, password);

            var response = await Client.UnblockUser(userId);

            return response.StatusCode;
        }

       
        [Test]
        public async Task Block_User_HasBecomeBlocked()
        {
            await LoginAsAdmin(Client);

            await Client.BlockUser(DEFAULT_USER_ID.ToString());
            var users = await GetUsers();

            users.Should().NotBeNull();
            var user = users!.FirstOrDefault(u => u.Id == DEFAULT_USER_ID);
            user.Should().NotBeNull();
            user!.IsBlocked.Should().BeTrue();
        }

        [Test]
        public async Task Unblock_User_HasBecomeUnblocked()
        {
            await LoginAsAdmin(Client);
            await Client.BlockUser(DEFAULT_USER_ID.ToString());

            await Client.UnblockUser(DEFAULT_USER_ID.ToString());
            var users = await GetUsers();

            users.Should().NotBeNull();
            var user = users!.FirstOrDefault(u => u.Id == DEFAULT_USER_ID);
            user.Should().NotBeNull();
            user!.IsBlocked.Should().BeFalse();
        }

        private static IEnumerable<TestCaseData> AccessGetUsersData()
        {
            yield return new TestCaseData(ADMIN_USERNAME, ADMIN_PASSWORD).Returns(HttpStatusCode.OK);
            yield return new TestCaseData(DEFAULT_USER_USERNAME, DEFAULT_USER_PASSWORD).Returns(HttpStatusCode.Forbidden);
            yield return new TestCaseData(null, null).Returns(HttpStatusCode.Unauthorized);
        }

        private static IEnumerable<TestCaseData> BlockAndUnblockUserData()
        {
            yield return new TestCaseData(ADMIN_USERNAME, ADMIN_PASSWORD, DEFAULT_USER_ID.ToString()).Returns(HttpStatusCode.OK);
            yield return new TestCaseData(ADMIN_USERNAME, ADMIN_PASSWORD, NON_EXISTENT_USER_ID.ToString()).Returns(HttpStatusCode.NotFound);
            yield return new TestCaseData(ADMIN_USERNAME, ADMIN_PASSWORD, "asdafg").Returns(HttpStatusCode.NotFound);
            yield return new TestCaseData(DEFAULT_USER_USERNAME, DEFAULT_USER_PASSWORD, DEFAULT_USER_ID.ToString()).Returns(HttpStatusCode.Forbidden);
            yield return new TestCaseData(null, null, DEFAULT_USER_ID.ToString()).Returns(HttpStatusCode.Unauthorized);
        }

        private async Task<List<User>?> GetUsers()
        {
            var response = await Client.GetUsers();
            return await response.GetContentAsync<List<User>>();
        }
    }
}

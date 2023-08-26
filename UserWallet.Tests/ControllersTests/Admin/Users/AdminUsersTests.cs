using UserWallet.Models;

namespace UserWallet.Tests.ControllersTests.Admin
{
    public class AdminUsersTests: BaseControllerTest
    {
        private const int NON_EXISTENT_USER_ID = 999;

        [Test]
        public async Task GetUsers_AsAdmin_Success()
        {
            await LoginAsAdmin(Client);

            var response = await Client.GetUsers();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task GetUsers_Anonymous_Unauthorized()
        {
            var response = await Client.GetUsers();

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task GetUsers_AsUser_Forbidden()
        {
            await LoginAsUser(Client);

            var response = await Client.GetUsers();

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Test]
        public async Task GetUsers_AsAdmin_ReturnsCorrectUsersCount()
        {
            await LoginAsAdmin(Client);
            
            var users = await GetUsers();

            users.Should().NotBeNull();
            users!.Should().HaveCount(DEFAULT_USERS_COUNT);
        }

        [Test]
        public async Task Block_User_Success()
        {
            await LoginAsAdmin(Client);

            var response = await Client.BlockUser(DEFAULT_USER_ID.ToString());

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Block_AsUser_Forbidden()
        {
            await LoginAsUser(Client);

            var response = await Client.BlockUser(DEFAULT_USER_ID.ToString());

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Test]
        public async Task Block_AsAnonymous_Unauthorized()
        {
            var response = await Client.BlockUser(DEFAULT_USER_ID.ToString());

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
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
        public async Task Block_NonExistenUser_NotFound()
        {
            await LoginAsAdmin(Client);

            var response = await Client.BlockUser(NON_EXISTENT_USER_ID.ToString());

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Block_NotIntUserId_NotFound()
        {
            await LoginAsAdmin(Client);

            var response = await Client.BlockUser("asdfg");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Unblock_AsAdmin_Success()
        {
            await LoginAsAdmin(Client);

            var response = await Client.UnblockUser(DEFAULT_USER_ID.ToString());

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Unblock_AsUser_Forbidden()
        {
            await LoginAsUser(Client);

            var response = await Client.UnblockUser(DEFAULT_USER_ID.ToString());

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Test]
        public async Task Unblock_AsAnonymous_Unauthorized()
        {
            var response = await Client.UnblockUser(DEFAULT_USER_ID.ToString());

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task Unblock_NonExistenUser_NotFound()
        {
            await LoginAsAdmin(Client);

            var response = await Client.UnblockUser(NON_EXISTENT_USER_ID.ToString());

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Unblock_NotIntUserId_NotFound()
        {
            await LoginAsAdmin(Client);

            var response = await Client.UnblockUser("asdfg"); ;

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
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

        private async Task<List<User>?> GetUsers()
        {
            var response = await Client.GetUsers();
            return await response.GetContentAsync<List<User>>();
        }
    }
}

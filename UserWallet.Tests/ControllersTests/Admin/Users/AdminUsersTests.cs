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

            var response = await Client.GetAsync("admin/users");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task GetUsers_Anonymous_Unauthorized()
        {
            var response = await Client.GetAsync("admin/users");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task GetUsers_AsUser_Forbidden()
        {
            await LoginAsUser(Client);

            var response = await Client.GetAsync("admin/users");

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

            var response = await BlockUser(DEFAULT_USER_ID);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Block_AsUser_Forbidden()
        {
            await LoginAsUser(Client);

            var response = await BlockUser(DEFAULT_USER_ID);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Test]
        public async Task Block_AsAnonymous_Unauthorized()
        {
            var response = await BlockUser(DEFAULT_USER_ID);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task Block_User_HasBecomeBlocked()
        {
            await LoginAsAdmin(Client);

            await BlockUser(DEFAULT_USER_ID);
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

            var response = await BlockUser(NON_EXISTENT_USER_ID);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Block_NotIntUserId_NotFound()
        {
            await LoginAsAdmin(Client);

            var response = await Client.PatchAsJsonAsync($"admin/users/block/asdfg", "");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Unblock_AsAdmin_Success()
        {
            await LoginAsAdmin(Client);

            var response = await UnblockUser(DEFAULT_USER_ID);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Unblock_AsUser_Forbidden()
        {
            await LoginAsUser(Client);

            var response = await UnblockUser(DEFAULT_USER_ID);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Test]
        public async Task Unblock_AsAnonymous_Unauthorized()
        {
            var response = await UnblockUser(DEFAULT_USER_ID);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task Unblock_NonExistenUser_NotFound()
        {
            await LoginAsAdmin(Client);

            var response = await UnblockUser(NON_EXISTENT_USER_ID);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Unblock_NotIntUserId_NotFound()
        {
            await LoginAsAdmin(Client);

            var response = await Client.PatchAsJsonAsync($"admin/users/unblock/asdfg", "");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Unblock_User_HasBecomeUnblocked()
        {
            await LoginAsAdmin(Client);
            await BlockUser(DEFAULT_USER_ID);

            await UnblockUser(DEFAULT_USER_ID);
            var users = await GetUsers();

            users.Should().NotBeNull();
            var user = users!.FirstOrDefault(u => u.Id == DEFAULT_USER_ID);
            user.Should().NotBeNull();
            user!.IsBlocked.Should().BeFalse();
        }

        private async Task<HttpResponseMessage> BlockUser(int userId)
            => await Client.PatchAsJsonAsync($"admin/users/block/{userId}", "");

        private async Task<HttpResponseMessage> UnblockUser(int userId)
            => await Client.PatchAsJsonAsync($"admin/users/unblock/{userId}", "");

        private async Task<List<User>?> GetUsers()
        {
            var response = await Client.GetAsync("admin/users");
            return await response.GetContentAsync<List<User>>();
        }
    }
}

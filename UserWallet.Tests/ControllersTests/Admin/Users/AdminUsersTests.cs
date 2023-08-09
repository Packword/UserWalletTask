using UserWallet.Models;

namespace UserWallet.Tests.ControllersTests.Admin
{
    public class AdminUsersTests: BaseControllerTest
    {
        private const int NON_EXISTEN_USER_ID = 999;

        [Test]
        public async Task GetUsers_AsAdmin_Success()
        {
            await LoginAsAdmin(_client);

            var response = await _client.GetAsync("admin/users");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task GetUsers_Anonymous_Unauthorized()
        {
            var response = await _client.GetAsync("admin/users");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task GetUsers_AsUser_Forbidden()
        {
            await LoginAsUser(_client);

            var response = await _client.GetAsync("admin/users");

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Test]
        public async Task GetUsers_AsAdmin_ReturnsCorrectUsersCount()
        {
            await LoginAsAdmin(_client);
            
            var users = await GetUsers();

            users.Should().NotBeNull();
            users!.Should().HaveCount(DEFAULT_USERS_COUNT);
        }

        [Test]
        public async Task Block_User_Success()
        {
            await LoginAsAdmin(_client);

            var response = await BlockUser(DEFAULT_USER_ID);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Block_AsUser_Forbidden()
        {
            await LoginAsUser(_client);

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
            await LoginAsAdmin(_client);

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
            await LoginAsAdmin(_client);

            var response = await BlockUser(NON_EXISTEN_USER_ID);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Block_NotIntUserId_NotFound()
        {
            await LoginAsAdmin(_client);

            var response = await _client.PatchAsJsonAsync($"admin/users/block/asdfg", "");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Unblock_User_Success()
        {
            await LoginAsAdmin(_client);

            var response = await UnblockUser(DEFAULT_USER_ID);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Unblock_AsUser_Forbidden()
        {
            await LoginAsUser(_client);

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
            await LoginAsAdmin(_client);

            var response = await UnblockUser(NON_EXISTEN_USER_ID);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Unblock_NotIntUserId_NotFound()
        {
            await LoginAsAdmin(_client);

            var response = await _client.PatchAsJsonAsync($"admin/users/unblock/asdfg", "");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Unblock_User_HasBecomeUnblocked()
        {
            await LoginAsAdmin(_client);
            await BlockUser(DEFAULT_USER_ID);

            await UnblockUser(DEFAULT_USER_ID);
            var users = await GetUsers();

            users.Should().NotBeNull();
            var user = users!.FirstOrDefault(u => u.Id == DEFAULT_USER_ID);
            user.Should().NotBeNull();
            user!.IsBlocked.Should().BeFalse();
        }

        private async Task<HttpResponseMessage> BlockUser(int userId)
            => await _client.PatchAsJsonAsync($"admin/users/block/{userId}", "");

        private async Task<HttpResponseMessage> UnblockUser(int userId)
            => await _client.PatchAsJsonAsync($"admin/users/unblock/{userId}", "");

        private async Task<List<User>?> GetUsers()
        {
            var response = await _client.GetAsync("admin/users");
            return await response.Content.ReadFromJsonAsync<List<User>>(TestOptions.JSON_OPTIONS);
        }
    }
}

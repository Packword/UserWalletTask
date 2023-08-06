namespace UserWallet.Tests.ControllersTests.Admin
{
    public class AdminUsersPositiveTests: BaseControllerTest
    {
        [Test]
        public async Task GetUsers_AsAdmin_Success()
        {
            await _authServiceHelper.LoginAsAdmin();
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "admin/users");

            var response = await _client.SendAsync(requestMessage);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var users = await GetUsers();
            users.Count.Should().Be(TestData.DEFAULT_USERS_COUNT);
        }

        [Test]
        public async Task GetUsers_AsAdmin_ReturnsCorrectUsersCount()
        {
            await _authServiceHelper.LoginAsAdmin();
            
            var users = await GetUsers();

            users.Count.Should().Be(TestData.DEFAULT_USERS_COUNT);
        }

        [Test]
        public async Task Block_User_Success()
        {
            await _authServiceHelper.LoginAsAdmin();

            var response = await BlockUser(TestData.DEFAULT_USER_ID);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Block_User_HasBecomeBlocked()
        {
            await _authServiceHelper.LoginAsAdmin();

            await BlockUser(TestData.DEFAULT_USER_ID);
            var users = await GetUsers();

            users.First(u => u.Id == TestData.DEFAULT_USER_ID).IsBlocked.Should().Be(true);
        }

        [Test]
        public async Task Unblock_User_Success()
        {
            await _authServiceHelper.LoginAsAdmin();
            await BlockUser(TestData.DEFAULT_USER_ID);

            var response = await UnblockUser(TestData.DEFAULT_USER_ID);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var users = await GetUsers();
            users.First(u => u.Id == TestData.DEFAULT_USER_ID).IsBlocked.Should().Be(false);
        }

        [Test]
        public async Task Unblock_User_HasBecomeUnblocked()
        {
            await _authServiceHelper.LoginAsAdmin();
            await BlockUser(TestData.DEFAULT_USER_ID);

            await UnblockUser(TestData.DEFAULT_USER_ID);
            var users = await GetUsers();

            users.First(u => u.Id == TestData.DEFAULT_USER_ID).IsBlocked.Should().Be(false);
        }

        private async Task<HttpResponseMessage> BlockUser(int userId)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Patch, $"admin/users/block/{userId}");
            return await _client.SendAsync(requestMessage);
        }

        private async Task<HttpResponseMessage> UnblockUser(int userId)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Patch, $"admin/users/unblock/{userId}");
            return await _client.SendAsync(requestMessage);
        }

        private async Task<List<User>> GetUsers()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "admin/users");
            var response = await _client.SendAsync(requestMessage);
            string content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<User>>(content, TestData.JSON_OPTIONS)!;
        }
    }
}

namespace UserWallet.Tests.ControllersTests.Admin
{
    public class AdminUsersPositiveTests: AdminTest
    {
        [Test]
        public async Task PositiveGetUsersTest()
        {
            var users = await GetUsers();
            users.Count.Should().Be(TestData.DEFAULT_USERS_COUNT);
        }

        [Test]
        public async Task PositiveBlockUserTest()
        {
            var response = await BlockUser(TestData.DEFAULT_USER_ID);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var users = await GetUsers();
            users.First(u => u.Id == TestData.DEFAULT_USER_ID).IsBlocked.Should().Be(true);
        }

        [Test]
        public async Task PositiveUnblockUserTest()
        {
            var response = await BlockUser(TestData.DEFAULT_USER_ID);
            response = await UnblockUser(TestData.DEFAULT_USER_ID);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
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

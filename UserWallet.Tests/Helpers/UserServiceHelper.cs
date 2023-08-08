namespace UserWallet.Tests.Helpers
{
    public static class UserServiceHelper
    {
        public static User CreateUser(string username, string password, string role, bool isBlocked)
            => new User
            {
                Username = username,
                Password = password,
                Role = role,
                IsBlocked = isBlocked
            };
    }
}

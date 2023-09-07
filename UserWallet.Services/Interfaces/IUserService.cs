namespace UserWallet.Interfaces
{
    public interface IUserService
    {
        public List<User> GetUsers();
        public bool AddUser(string userName, string password);
        public void ChangePassword(int userId, string newPassword);
        public User? GetUserById(int userId);
        public User? GetUserByName(string? userName);
        public User? GetUserByNameAndPassword(string username, string password);
        public bool BlockUser(int userId);
        public bool UnblockUser(int userId);
    }
}

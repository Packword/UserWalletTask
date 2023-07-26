namespace UserWallet.Interfaces
{
    public interface IUserService
    {
        public List<User> GetUsers();
        public bool AddUser(string userName, string password);
        public void ChangePassword(string newPassword, int userId);
        public User? GetUserById(int userId);
        public User? GetUserByNameAndPassword(string username, string password);
        public bool BlockUser(int userId);
        public bool UnblockUser(int userId);
    }
}

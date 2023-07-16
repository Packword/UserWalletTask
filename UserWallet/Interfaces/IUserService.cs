using UserWallet.Models;

namespace UserWallet.Interfaces
{
    public interface IUserService
    {
        public List<User>? GetUsers();
        public (bool result, string msg) AddUser(string userName, string password);
        public void ChangePassword(string newPassword, HttpContext context);
        public User GetUserById(int userId);
        public User? GetUserByNameAndPassword(string username, string password);
        public void AddBalance(int userId, string currency, decimal amount);
    }
}

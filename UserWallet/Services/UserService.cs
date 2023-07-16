using System.Security.Claims;
using System.Text.Json;
using UserWallet.Interfaces;
using UserWallet.Models;

namespace UserWallet.Services
{
    public class UserService: IUserService
    {
        private List<User>? users;
        public UserService()
        {
            if (users is null)
            {
                try
                {
                    using (FileStream fs = new FileStream("./Data/Users.json", FileMode.Open))
                    {
                        users = JsonSerializer.Deserialize<List<User>>(fs);
                    }
                }
                catch (Exception e)
                {
                    throw new FileLoadException("\n Ошибка при чтении файла\n" + e.Message);
                }
            }
        }

        public List<User>? GetUsers()
        {
            return users;
        }
        public (bool result, string msg) AddUser(string userName, string password)
        {

            if (GetUserByNameAndPassword(userName, password) is not null)
                return (false, "A user with this username and password already exists");

            User user = new User()
            {
                Id = GetNewId(),
                Username = userName,
                Password = password,
                Role = "User"
            };

            users.Add(user);

            SerializeUsers();
            return (true, "");
        }

        public void ChangePassword(string newPassword, HttpContext context)
        {
            int userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            User user = GetUserById(userId);
            user.Password = newPassword;
            SerializeUsers();
        }

        public User? GetUserById(int userId)
        {
            return users.First(u => u.Id == userId);
        }

        public User? GetUserByNameAndPassword(string username, string password)
        {
            return users.FirstOrDefault(u => Equals(u.Username, username) && Equals(u.Password, password));
        }

        public void AddBalance(int userId, string currency, decimal amount)
        {
            User user = GetUserById(userId)!;
            if (user.Balances.ContainsKey(currency))
                user.Balances[currency] += amount;
            else
            {
                user.Balances.Add(currency, amount);
            }
            SerializeUsers();
        }

        private int GetNewId()
        {
            return users[users.Count - 1].Id + 1;
        }

        private void SerializeUsers()
        {
            using (FileStream fs = new FileStream("./Data/Users.json", FileMode.Truncate))
            {
                JsonSerializer.Serialize(fs, users);
            }
        }
    }
}

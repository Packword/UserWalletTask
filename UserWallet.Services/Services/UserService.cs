using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using UserWallet.Interfaces;
using UserWallet.Models;

namespace UserWallet.Services
{
    public class UserService: IUserService
    {
        private readonly ApplicationDbContext _db;
        public UserService(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<User> GetUsers()
        {
            return _db.Users.ToList();
        }

        public bool BlockUser(int userId)
        {
            User? user = GetUserById(userId);
            if (user is null)
                return false;

            user.IsBlocked = true;
            _db.SaveChanges();
            return true;
        }
        public bool UnblockUser(int userId)
        {
            User? user = GetUserById(userId);
            if (user is null)
                return false;

            user.IsBlocked = false;
            _db.SaveChanges();
            return true;
        }

        public (bool result, string msg) AddUser(string userName, string password)
        {

            if (GetUserByNameAndPassword(userName, password) is not null)
                return (false, "A user with this username and password already exists");

            User user = new User()
            {
                Username = userName,
                Password = password,
                IsBlocked = false,
                Role = "User"
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return (true, "");
        }

        public void ChangePassword(string newPassword, HttpContext context)
        {
            int userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            User user = GetUserById(userId)!;
            user.Password = newPassword;
            _db.SaveChanges();
        }

        public User? GetUserById(int userId)
        {
            return _db.Users.Include(u => u.Balances).FirstOrDefault(u => u.Id == userId);
        }

        public User? GetUserByNameAndPassword(string username, string password)
        {
            return _db.Users.Include(u => u.Balances).FirstOrDefault(u => Equals(u.Username, username) && Equals(u.Password, password));
        }

        public void AddBalance(int userId, string currency, decimal amount)
        {
            UserBalance? balance = _db.UserBalances.FirstOrDefault(b => Equals(currency, b.CurrencyId) && Equals(b.UserId, userId));
            if (balance is not null)
                balance.Amount += amount;
            else
            {
                 _db.UserBalances.Add( new UserBalance {
                    UserId = userId,
                    CurrencyId = currency,
                    Amount = amount
                });
            }
            _db.SaveChanges();
        }
    }
}

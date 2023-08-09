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
            => _db.Users.ToList();

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

        public bool AddUser(string userName, string password)
        {
            if (_db.Users.FirstOrDefault(u => u.Username == userName) is not null)
                return false;

            User user = new User()
            {
                Username = userName,
                Password = password,
                IsBlocked = false,
                Role = UsersRole.USER
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return true;
        }

        public void ChangePassword(int userId, string newPassword)
        {
            User user = GetUserById(userId)!;
            user.Password = newPassword;
            _db.SaveChanges();
        }

        public User? GetUserById(int userId)
            => _db.Users.FirstOrDefault(u => u.Id == userId);

        public User? GetUserByNameAndPassword(string username, string password)
            => _db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
    }
}

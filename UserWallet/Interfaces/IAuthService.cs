using UserWallet.Models;

namespace UserWallet.Interfaces
{
    public interface IAuthService
    {
        public Task<bool> Login(User? user, HttpContext context);
    }
}

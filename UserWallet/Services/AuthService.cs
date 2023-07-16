using UserWallet.Interfaces;
using UserWallet.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace UserWallet.Services
{
    public class AuthService: IAuthService
    {
        public async Task<bool> Login(User? user, HttpContext context)
        {
            
            if (user is null)
                return false;

            else
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };

                ClaimsIdentity identity = new ClaimsIdentity(claims, "Cookies");
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return true;
            }
        }
    }
}

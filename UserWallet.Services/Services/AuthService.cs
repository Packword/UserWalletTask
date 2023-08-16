namespace UserWallet.Services
{
    public class AuthService: IAuthService
    {
        public ClaimsPrincipal MakeClaims(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Role, user.Role),
                new(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            return new(identity);
        }
    }
}

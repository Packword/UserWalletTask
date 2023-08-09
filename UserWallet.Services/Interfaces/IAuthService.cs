namespace UserWallet.Interfaces
{
    public interface IAuthService
    {
        public ClaimsPrincipal GenerateClaims(User user);
    }
}

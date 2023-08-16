namespace UserWallet.Interfaces
{
    public interface IAuthService
    {
        public ClaimsPrincipal MakeClaims(User user);
    }
}

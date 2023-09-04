namespace UserWallet.Services.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetId(this ClaimsPrincipal principal)
            => int.TryParse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId) ? userId : null;

        public static string? GetName(this ClaimsPrincipal principal)
           => principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
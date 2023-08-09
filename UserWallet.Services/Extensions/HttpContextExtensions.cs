namespace UserWallet.Services.Extensions
{
    public static class HttpContextExtensions
    {
        public static int GetCurrentUserId(this HttpContext context)
            => int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}

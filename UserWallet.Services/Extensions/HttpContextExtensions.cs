namespace UserWallet.Services.Extensions
{
    public static class HttpContextExtensions
    {
        public static int? GetCurrentUserId(this HttpContext context)
            => context.User.GetId();
    }
}

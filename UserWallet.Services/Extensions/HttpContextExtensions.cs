namespace UserWallet.Services.Extensions
{
    public static class HttpContextExtensions
    {
        public static int? GetCurrentUserId(this HttpContext context)
            => int.TryParse(context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId) ? userId : null;
    }
}

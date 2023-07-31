namespace UserWallet.Services
{
    public class HttpContextService: IHttpContextService
    {
        public int GetCurrentUserId(HttpContext context)
            => int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}

using Microsoft.AspNetCore.Http;

namespace UserWallet.Interfaces
{
    public interface IHttpContextService
    {
        public int GetCurrentUserId(HttpContext context);
    }
}

namespace UserWallet.Tests.Helpers
{
    public abstract class BaseApiHelper
    {
        protected readonly HttpClient _client;
        public BaseApiHelper(HttpClient client)
        {
            _client = client;
        }
    }
}

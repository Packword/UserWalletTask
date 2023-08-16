namespace UserWallet.Tests.Extensions
{
    public static class HttpResponseExtension
    {
        public async static Task<T?> GetContentAsync<T>(this HttpResponseMessage response)
            => await response.Content.ReadFromJsonAsync<T>(TestOptions.JSON_OPTIONS);
    }
}

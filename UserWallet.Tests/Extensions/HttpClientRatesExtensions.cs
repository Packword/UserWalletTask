namespace UserWallet.Tests.Extensions
{
    public static class HttpClientRatesExtension
    {
        public async static Task<HttpResponseMessage> GetRates(this HttpClient client)
            => await client.GetAsync("/rates");
    }
}

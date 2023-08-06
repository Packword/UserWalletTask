namespace UserWallet.Tests.ControllersTests.Rates
{
    public class RatesPositiveTests: BaseControllerTest
    {
        [Test]
        public async Task GetRates_AvailableCurrency_Success()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/rates");

            var response = await _client.SendAsync(requestMessage);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task GetRates_AvailableCurrency_MustContain()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/rates");

            var response = await _client.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();
            var rates = JsonSerializer.Deserialize<CurrentRateDTO>(content, TestData.JSON_OPTIONS);

            rates!.Rates.ContainsKey(TestData.CRYPTO_CURRENCY_ID).Should().Be(true);
            rates.Rates.ContainsKey(TestData.FIAT_CURRENCY_ID).Should().Be(true);
        }
    }
}

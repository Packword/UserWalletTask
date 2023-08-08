namespace UserWallet.Tests.ControllersTests.Rates
{
    public class RatesTests: BaseControllerTest
    {
        [Test]
        public async Task GetRates_AvailableCurrency_Success()
        {
            var response = await _client.GetAsync("/rates");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task GetRates_AvailableCurrency_MustContain()
        {
            var response = await _client.GetAsync("/rates");
            var rates = await response.Content.ReadFromJsonAsync<CurrentRateDTO>(TestOptions.JSON_OPTIONS);

            rates.Should().NotBeNull();
            rates!.Rates.Should().ContainKey(CRYPTO_CURRENCY_ID); 
            rates.Rates.Should().ContainKey(FIAT_CURRENCY_ID);

        }
    }
}

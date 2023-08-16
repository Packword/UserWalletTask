namespace UserWallet.Tests.ControllersTests.Rates
{
    public class RatesTests: BaseControllerTest
    {
        [Test]
        public async Task GetRates_AvailableCurrency_Success()
        {
            var response = await Client.GetAsync("/rates");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task GetRates_AvailableCurrency_MustContain()
        {
            var response = await Client.GetAsync("/rates");
            var rates = await response.GetContentAsync<CurrentRateDTO>();

            rates.Should().NotBeNull();
            rates!.Rates.Should().ContainKey(CRYPTO_CURRENCY_ID); 
            rates.Rates.Should().ContainKey(FIAT_CURRENCY_ID);
            rates.Rates[CRYPTO_CURRENCY_ID].Should().NotBe(0);
            rates.Rates[FIAT_CURRENCY_ID].Should().NotBe(0);

        }
    }
}

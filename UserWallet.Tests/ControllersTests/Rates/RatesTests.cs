namespace UserWallet.Tests.ControllersTests.Rates
{
    public class RatesTests: BaseControllerTest
    {
        [Test]
        public async Task GetRates_AvailableCurrency_MustContain()
        {
            var response = await Client.GetRates();
            var rates = await response.GetContentAsync<CurrentRateDTO>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            rates.Should().NotBeNull();
            rates!.Rates.Should().ContainKey(CRYPTO_CURRENCY_ID); 
            rates.Rates.Should().ContainKey(FIAT_CURRENCY_ID);
            rates.Rates[CRYPTO_CURRENCY_ID].Should().NotBe(0);
            rates.Rates[FIAT_CURRENCY_ID].Should().NotBe(0);
        }
    }
}

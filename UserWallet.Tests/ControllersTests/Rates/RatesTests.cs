namespace UserWallet.Tests.ControllersTests.Rates
{
    public class RatesTests: BaseControllerTest
    {
        private readonly Currency newCurrency = new Currency() { Id = "usd", IsAvailable = true, Type = CurrencyType.Fiat };

        [Test]
        public async Task GetRates_AfterChangeCurrency_MustContainNewCurrency()
        {
            var sp = Factory.Services;
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ApplicationDbContext>();
            var responseBefore = await Client.GetRates();
            var ratesBefore = await responseBefore.GetContentAsync<CurrentRateDTO>();
            responseBefore.StatusCode.Should().Be(HttpStatusCode.OK);
            AssertDefaultRates(ratesBefore);
            ratesBefore!.Rates.Should().NotContainKey(newCurrency.Id);

            db.Currencies.Add(newCurrency);
            db.SaveChanges();
            await Task.Delay(EXCHANGE_UPDATE_INTERVAL);
            var responseAfter = await Client.GetRates();
            var ratesAfter = await responseAfter.GetContentAsync<CurrentRateDTO>();

            responseAfter.StatusCode.Should().Be(HttpStatusCode.OK);
            AssertDefaultRates(ratesAfter);
            ratesAfter!.Rates.Should().ContainKey(newCurrency.Id);
            ratesAfter.Rates[newCurrency.Id].Should().NotBe(0);
        }

        [Test]
        public async Task GetRates_AvailableCurrency_MustContain()
        {
            var response = await Client.GetRates();
            var rates = await response.GetContentAsync<CurrentRateDTO>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            AssertDefaultRates(rates);
        }

        private static void AssertDefaultRates(CurrentRateDTO? rates)
        {
            rates.Should().NotBeNull();
            rates!.Rates.Should().ContainKey(CRYPTO_CURRENCY_ID);
            rates.Rates.Should().ContainKey(FIAT_CURRENCY_ID);
            rates.Rates[CRYPTO_CURRENCY_ID].Should().NotBe(0);
            rates.Rates[FIAT_CURRENCY_ID].Should().NotBe(0);
        }
    }
}

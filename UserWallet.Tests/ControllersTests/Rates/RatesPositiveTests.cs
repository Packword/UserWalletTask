namespace UserWallet.Tests.ControllersTests.Rates
{
    public class RatesPositiveTests: BaseControllerTest
    {
        [Test]
        public async Task PositiveGetRatesControllerTest()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/rates");
            var response = await _client.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();
            var rates = JsonSerializer.Deserialize<CurrentRateDTO>(content, TestData.JSON_OPTIONS);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            rates!.Rates.ContainsKey("btc").Should().Be(true);
            rates.Rates.ContainsKey("rub").Should().Be(true);
            rates.Rates.ContainsKey("usd").Should().Be(false);
        }
    }
}

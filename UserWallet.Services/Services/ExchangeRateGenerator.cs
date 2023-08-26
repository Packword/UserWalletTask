using Microsoft.Extensions.DependencyInjection;

namespace UserWallet.Services
{
    public class ExchangeRateGenerator : IHostedService
    {
        private readonly IOptionsMonitor<ExchangeRateGeneratorOptions> _config;
        private readonly IServiceProvider _serviceProvider;
        private readonly Random rnd = new();
        private readonly Dictionary<string, decimal> rates = new();

        private List<Currency>? currencies;

        public ExchangeRateGenerator(IOptionsMonitor<ExchangeRateGeneratorOptions> config, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _config = config;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            UpdateRatesAsync(cancellationToken);
            return Task.CompletedTask;
        }

        private void InitRates()
        {
            if (currencies is not null)
            {
                foreach (var currency in currencies)
                {
                    rates.Add(currency.Id, rnd.Next(80, 120));
                }
            }
        }

        private void InitCurrencies()
        {
            using var scope = _serviceProvider.CreateScope();
            var currencyService = scope.ServiceProvider.GetRequiredService<ICurrencyService>();
            currencies = currencyService.GetCurrencies();
        }

        private async Task UpdateRatesAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (rates.Keys.Count is 0)
                {
                    InitCurrencies();
                    InitRates();
                }
                else
                {
                    foreach (string key in rates.Keys)
                    {
                        rates[key] *= 1 + 0.05m * rnd.Next(-1, 2);
                    }
                }
                await Task.Delay(_config.CurrentValue.UpdateInterval, stoppingToken);
            }
        }

        public Dictionary<string, decimal> GetCurrentRates()
        {
            if(rates.Count is 0)
            {
                InitCurrencies();
                InitRates();
            }
            return rates;
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}

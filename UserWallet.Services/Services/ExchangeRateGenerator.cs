using Microsoft.Extensions.DependencyInjection;

namespace UserWallet.Services
{
    public class ExchangeRateGenerator : IHostedService
    {
        private readonly IOptionsMonitor<ExchangeRateGeneratorOptions> _config;
        private readonly IServiceProvider _serviceProvider;
        private readonly Random rnd = new();
        private Task? _task;
        private CancellationTokenSource _cts = new();

        private List<Currency>? currencies;
        private Dictionary<string, decimal> rates = new();

        public ExchangeRateGenerator(IOptionsMonitor<ExchangeRateGeneratorOptions> config, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _config = config;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = new CancellationTokenSource();
            _task = UpdateRatesAsync(_cts.Token);
            return Task.CompletedTask;
        }

        private async Task UpdateRatesAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    UpdateRatesIfNeeded();
                    if (rates.Keys is { Count: > 0 } keys)
                    {
                        foreach (var key in keys)
                        {
                            rates[key] *= 1 + 0.05m * rnd.Next(-1, 2);
                        }
                    }
                    await Task.Delay(_config.CurrentValue.UpdateInterval, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }

        public Dictionary<string, decimal> GetCurrentRates()
        {
            UpdateRatesIfNeeded();
            return rates;
        }

        private void UpdateRatesIfNeeded()
        {
            UpdateCurrencies();
            if(currencies is not null && rates.Count != currencies.Count)
                UpdateRates();
        }

        private void UpdateCurrencies()
        {
            using var scope = _serviceProvider.CreateScope();
            var currencyService = scope.ServiceProvider.GetRequiredService<ICurrencyService>();
            currencies = currencyService.GetCurrencies();
        }

        private void UpdateRates()
        {
            var currenciesIds = currencies!.Select(x => x.Id);
            rates = rates.Where(r => currenciesIds.Contains(r.Key)).ToDictionary(k => k.Key, v => v.Value);
            foreach (var currency in currenciesIds)
            {
                if(!rates.ContainsKey(currency))
                    rates.Add(currency, rnd.Next(80, 120));
            }
        }


        public async Task StopAsync(CancellationToken cancellationToken)
        { 
            _cts.Cancel();
            await (_task ?? Task.CompletedTask);
        }
    }
}

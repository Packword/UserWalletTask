using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Text.Json;
using UserWallet.Models;
using UserWallet.OptionsModels;

namespace UserWallet.Services
{
    public class ExchangeRateGenerator : IHostedService
    {
        private readonly IOptionsMonitor<ExchangeRateOptions> _config;
        IDbContextFactory<ApplicationDbContext> _contextFactory;
        private Dictionary<string, decimal> currentRates = new Dictionary<string, decimal>();
        private List<Currency>? currencies;

        public ExchangeRateGenerator(IOptionsMonitor<ExchangeRateOptions> config, IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
            _config = config;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Random rnd = new Random();
            using (var context = _contextFactory.CreateDbContext())
            {
                currencies = context.Currencies.ToList();
            }

            foreach(var currency in currencies)
            {
                currentRates.Add(currency.Id, rnd.Next(80, 120));
            }

            UpdateRatesAsync(cancellationToken);
            return Task.CompletedTask;
        }

        private async Task UpdateRatesAsync(CancellationToken stoppingToken)
        {
            Random rnd = new Random();
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach(string key in currentRates.Keys)
                {
                    currentRates[key] += currentRates[key] * (decimal)0.05 * rnd.Next(-1, 2);
                }

                await Task.Delay(_config.CurrentValue.ChangeTime * 1000, stoppingToken);
            }
        }

        public List<Currency> GetCurrencies()
        {
            return currencies;
        }

        public Dictionary<string, decimal> GetCurrentRates()
        {
            return currentRates;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

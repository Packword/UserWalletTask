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
        private Task task;

        public ExchangeRateGenerator(IOptionsMonitor<ExchangeRateOptions> config, IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
            _config = config;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Random rnd = new Random();
            InitCurrencies();
            InitRates(rnd);

            task = UpdateRatesAsync(cancellationToken, rnd);
            return Task.CompletedTask;
        }

        private void InitRates(Random rnd)
        {
            foreach (var currency in currencies)
            {
                currentRates.Add(currency.Id, rnd.Next(80, 120));
            }
        }

        private void InitCurrencies()
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                currencies = context.Currencies.ToList();
            }
        }

        private async Task UpdateRatesAsync(CancellationToken stoppingToken, Random rnd)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach(string key in currentRates.Keys)
                {
                    currentRates[key] *= (1 + 0.05m * rnd.Next(-1, 2));
                }

                await Task.Delay(TimeSpan.FromSeconds(_config.CurrentValue.UpdateInterval), stoppingToken);
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

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await task;
        }
    }
}

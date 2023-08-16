namespace UserWallet.Services
{
    public class ExchangeRateGenerator : IHostedService
    {
        private readonly IOptionsMonitor<ExchangeRateGeneratorOptions> _config;
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly Random rnd = new();
        private readonly Dictionary<string, decimal> rates = new();

        private List<Currency> currencies = new();

        public ExchangeRateGenerator(IOptionsMonitor<ExchangeRateGeneratorOptions> config, IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
            _config = config;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            InitCurrencies();
            InitRates();

            UpdateRatesAsync(cancellationToken);
            return Task.CompletedTask;
        }

        private void InitRates()
        {
            foreach (var currency in currencies)
            {
                rates.Add(currency.Id, rnd.Next(80, 120));
            }
        }

        private void InitCurrencies()
        {
            using var context = _contextFactory.CreateDbContext();
            currencies = context.Currencies.ToList();
        }

        private async Task UpdateRatesAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (string key in rates.Keys)
                {
                    rates[key] *= 1 + 0.05m * rnd.Next(-1, 2);
                }   
                await Task.Delay(_config.CurrentValue.UpdateInterval, stoppingToken);
            }
        }

        public List<Currency> GetCurrencies()
            => currencies;

        public Dictionary<string, decimal> GetCurrentRates()
            => rates;

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}

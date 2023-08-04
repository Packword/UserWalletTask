namespace UserWallet.Services
{
    public class ExchangeRateGenerator : IHostedService
    {
        private readonly IOptionsMonitor<ExchangeRateGeneratorOptions> _config;
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly Random rnd = new Random();

        private Dictionary<string, decimal> currentRates = new Dictionary<string, decimal>();
        private List<Currency> currencies;
        private Task task;

        public ExchangeRateGenerator(IOptionsMonitor<ExchangeRateGeneratorOptions> config, IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
            _config = config;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            InitCurrencies();
            InitRates();

            task = UpdateRatesAsync(cancellationToken);
            return Task.CompletedTask;
        }

        private void InitRates()
        {
            foreach (var currency in currencies)
            {
                currentRates.Add(currency.Id, rnd.Next(80, 120));
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
                foreach (string key in currentRates.Keys)
                {
                    currentRates[key] *= 1 + 0.05m * rnd.Next(-1, 2);
                }   
                await Task.Delay(_config.CurrentValue.UpdateInterval, stoppingToken);
            }
        }

        public List<Currency> GetCurrencies()
            => currencies;

        public Dictionary<string, decimal> GetCurrentRates()
            => currentRates;

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}

namespace UserWallet.Tests.Helpers
{
    public class WebApplicationFactoryHelper
    {
        public WebApplicationFactory<Program> CreateFactoryWithInMemoryDb()
        {
            return new WebApplicationFactory<Program>().WithWebHostBuilder(
                    b =>
                    {
                        b.ConfigureServices(
                            c =>
                            {
                                var descriptor = c.SingleOrDefault(
                                    d => d.ServiceType ==
                                        typeof(DbContextOptions<ApplicationDbContext>));

                                if (descriptor != null)
                                {
                                    c.Remove(descriptor);
                                }

                                c.AddDbContextFactory<ApplicationDbContext>(options =>
                                    options.UseInMemoryDatabase("InMemoryDbForTesting"));
                            });
                    });
        }
    }
}

﻿using Microsoft.Extensions.Hosting;
using UserWallet.OptionsModels;
using UserWallet.Services;

namespace UserWallet.Tests.Helpers
{
    public static class WebApplicationFactoryHelper
    {
        public static WebApplicationFactory<Program> CreateFactoryWithInMemoryDb()
        {
            return new WebApplicationFactory<Program>().WithWebHostBuilder(
                    b =>
                    {
                        b.ConfigureServices(
                            c =>
                            {
                                var descriptor = c.SingleOrDefault(
                                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                                if (descriptor != null)
                                    c.Remove(descriptor);

                                descriptor = c.SingleOrDefault(
                                    d => d.ImplementationType == typeof(SeedDataFromJsonService));

                                if (descriptor != null)
                                    c.Remove(descriptor);

                                c.AddDbContextFactory<ApplicationDbContext>(options =>
                                    options.UseInMemoryDatabase("InMemoryDbForTesting"));
                            });
                    });
        }
    }
}

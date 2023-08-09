namespace UserWallet
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder.Services, builder.Configuration);
            
            var app = builder.Build();
            app.UseCors();
            using (var scope = app.Services.CreateScope())
            {
                SeedDataFromJson.Initialize(scope.ServiceProvider);
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
        {
            services.Configure<ExchangeRateGeneratorOptions>(configuration.GetSection("ExchangeRate"));

            services.AddCors(b =>
                b.AddDefaultPolicy(policy => policy.WithOrigins("http://localhost:63342")
                                       .AllowAnyHeader()
                                       .AllowAnyMethod()
                                       .AllowCredentials()));


            services.AddControllers().AddJsonOptions(options =>
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
            );
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
           o =>
           {
               o.Events.OnRedirectToLogin = context =>
               {
                   context.Response.StatusCode = 401;
                   return Task.CompletedTask;
               };
               o.Events.OnRedirectToAccessDenied = context =>
               {
                   context.Response.StatusCode = 403;
                   return Task.CompletedTask;
               };
           });
            services.AddAuthorization();
            services.AddScoped<IConvertToUsdService, ConvertToUsdService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IDepositFiatService, DepositFiatService>();
            services.AddScoped<IDepositCryptoService, DepositCryptoService>();
            services.AddScoped<IUserBalanceService, UserBalanceService>();
            services.AddSingleton<ExchangeRateGenerator>();
            services.AddSingleton<IHostedService, ExchangeRateGenerator>(serviceProvider => serviceProvider.GetRequiredService<ExchangeRateGenerator>());
            services.AddDbContextFactory<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("ApplicationDbContext")));
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }
    }
}
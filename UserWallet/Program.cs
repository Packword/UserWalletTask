using Microsoft.AspNetCore.Authentication.Cookies;
using UserWallet.Interfaces;
using UserWallet.OptionsModels;
using UserWallet.Services;

namespace UserWallet
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.Configure<ExchangeRateOptions>(builder.Configuration.GetSection("ExchangeRate"));

            builder.Services.AddControllers();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
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
            builder.Services.AddAuthorization();
            builder.Services.AddSingleton<IUserService, UserService>();
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<ExchangeRateGenerator>();
            builder.Services.AddSingleton<IHostedService>(p => p.GetRequiredService<ExchangeRateGenerator>());
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
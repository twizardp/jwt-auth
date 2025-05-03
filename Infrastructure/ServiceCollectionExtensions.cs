using Application.Services.Identity;
using Infrastructure.DbContext;
using Infrastructure.Services.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(option =>
            {
                var connStr = configuration.GetConnectionString("DefaultConnection");
                var serverVersion = MySqlServerVersion.AutoDetect(connStr);
                option.UseMySql(connStr, serverVersion, o => o.SchemaBehavior(MySqlSchemaBehavior.Ignore));
            }).AddTransient<ApplicationDbSeeder>();

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();
            return services;
        }
    }
}
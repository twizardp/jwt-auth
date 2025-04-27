using Infrastructure.DbContext;
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
            });

            return services;
        }
    }
}

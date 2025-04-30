using System.Reflection;
using Infrastructure.DbContext;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using WebApi.Permissions;

namespace WebApi;

public static class ServiceCollectionExtensions
{
    internal static IApplicationBuilder SeedDatabase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider.GetServices<ApplicationDbSeeder>();
        foreach (var service in services)
        {
            service.SeedDatabaseAsync().GetAwaiter().GetResult();
        }

        return app;
    }

    internal static IServiceCollection AddIdentitySettings(this IServiceCollection services)
    {
        services
            .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
            .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>()
            .AddIdentity<User, Role>(opt =>
            {
                opt.Password.RequiredLength = 6;
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    internal static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "jwt-auth API",
                Version = "v1",
                Description = ""
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }
}

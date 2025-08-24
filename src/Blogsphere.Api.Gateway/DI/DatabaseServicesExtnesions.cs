using Microsoft.EntityFrameworkCore;

namespace Blogsphere.Api.Gateway.DI;

public static class DatabaseServicesExtnesions
{
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration){
        // Add database configuration
        services.AddDbContext<ProxyConfigContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection") ?? 
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found."),
                o =>
                {
                    o.MigrationsAssembly("Blogsphere.Api.Gateway");
                    o.MigrationsHistoryTable("__EFMigrationsHistory", "blogsphere");
                    o.EnableRetryOnFailure(3);
                })
        );

        // Register repositories
        services.AddScoped<IProxyRouteRepository, ProxyRouteRepository>();
        services.AddScoped<IProxyClusterRepository, ProxyClusterRepository>();
        services.AddScoped<IProxyDestinationRepository, ProxyDestinationRepository>();
        services.AddScoped<IProxyHeaderRepository, ProxyHeaderRepository>();
        services.AddScoped<IProxyTransformRepository, ProxyTransformRepository>();
        services.AddScoped(typeof(ISubscriptionRepository<>), typeof(SubscriptionRepository<>));

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Seeder
        services.AddProxyConfigSeeder();
        
        return services;
    }
}

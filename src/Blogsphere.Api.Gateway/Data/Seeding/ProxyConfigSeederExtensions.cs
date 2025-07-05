namespace Blogsphere.Api.Gateway.Data.Seeding;

public static class ProxyConfigSeederExtensions
{
    public static IServiceCollection AddProxyConfigSeeder(this IServiceCollection services)
    {
        services.AddScoped<IProxyConfigSeeder, ProxyConfigSeeder>();
        return services;
    }

    public static async Task<IHost> SeedProxyConfigurationAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<IProxyConfigSeeder>();
        await seeder.SeedAsync();
        return host;
    }
} 
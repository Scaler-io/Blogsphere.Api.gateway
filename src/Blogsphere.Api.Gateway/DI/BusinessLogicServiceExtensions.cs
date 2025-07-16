using Blogsphere.Api.Gateway.Models.Mappings;
using Blogsphere.Api.Gateway.Services;
using Blogsphere.Api.Gateway.Services.Interfaces;

namespace Blogsphere.Api.Gateway.DI;

public static class BusinessLogicServiceExtensions
{
    public static IServiceCollection AddBusinessLogicServices(this IServiceCollection services){
        
        // Register Services
        services.AddTransient<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IProxyRouteService, ProxyRouteService>();
        services.AddScoped<IProxyClusterService, ProxyClusterService>();
        services.AddScoped<IProxyDestinationService, ProxyDestinationService>();

        // Add AutoMapper
        services.AddAutoMapper(typeof(ProxyMappingProfile));

        return services;
    }
}

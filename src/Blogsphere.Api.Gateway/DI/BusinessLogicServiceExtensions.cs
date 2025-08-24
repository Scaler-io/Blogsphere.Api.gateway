namespace Blogsphere.Api.Gateway.DI;

public static class BusinessLogicServiceExtensions
{
    public static IServiceCollection AddBusinessLogicServices(this IServiceCollection services){
        
        // Register Services
        services.AddTransient<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IProxyRouteService, ProxyRouteService>();
        services.AddScoped<IProxyClusterService, ProxyClusterService>();

        services.AddScoped<IApiProductManageService, ApiProductManageService>();
        services.AddScoped<ISubscribedApiManageService, SubscribedApiManageService>();
        services.AddScoped<ISubscriptionManageService , SubscriptionManageService>();
        services.AddScoped<ISubscriptionRetrievaService, SubscriptionRetrievalService>();
        
        // Add AutoMapper
        services.AddAutoMapper(typeof(ProxyMappingProfile).Assembly);

        return services;
    }
}

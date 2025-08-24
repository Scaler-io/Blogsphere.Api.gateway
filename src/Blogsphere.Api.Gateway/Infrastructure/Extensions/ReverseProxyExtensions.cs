using Yarp.ReverseProxy.Configuration;

namespace Blogsphere.Api.Gateway.Infrastructure.Extensions;

public static class ReverseProxyExtensions
{
    public static IReverseProxyBuilder AddDatabaseConfig(this IReverseProxyBuilder builder)
    {
        builder.Services.AddSingleton<DatabaseProxyConfigProvider>();
        builder.Services.AddSingleton<IProxyConfigProvider>(sp => sp.GetRequiredService<DatabaseProxyConfigProvider>());
        return builder;
    }
} 
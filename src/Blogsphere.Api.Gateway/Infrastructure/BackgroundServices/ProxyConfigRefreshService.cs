using Blogsphere.Api.Gateway.Infrastructure.Yarp;

namespace Blogsphere.Api.Gateway.Infrastructure.BackgroundServices;

public class ProxyConfigRefreshService(
    IServiceProvider serviceProvider,
    ILogger<ProxyConfigRefreshService> logger) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<ProxyConfigRefreshService> _logger = logger;
    private readonly TimeSpan _refreshInterval = TimeSpan.FromSeconds(30);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ProxyConfig refresh service is starting");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var configProvider = scope.ServiceProvider.GetRequiredService<DatabaseProxyConfigProvider>();
                
                _logger.LogInformation("Refreshing proxy configuration from database");
                configProvider.Update();
                _logger.LogInformation("Proxy configuration refreshed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while refreshing proxy configuration");
            }

            await Task.Delay(_refreshInterval, stoppingToken);
        }
    }
} 
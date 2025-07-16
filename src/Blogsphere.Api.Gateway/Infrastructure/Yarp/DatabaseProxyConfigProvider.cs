using Blogsphere.Api.Gateway.Data.Interfaces;
using Blogsphere.Api.Gateway.Entity;
using Blogsphere.Api.Gateway.Extensions;
using Microsoft.Extensions.Primitives;
using System.Linq.Expressions;
using Yarp.ReverseProxy.Configuration;

namespace Blogsphere.Api.Gateway.Infrastructure.Yarp;

public class DatabaseProxyConfigProvider : IProxyConfigProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;
    private IProxyConfig _config;
    private CancellationTokenSource _changeToken;

    public DatabaseProxyConfigProvider(
        IServiceProvider serviceProvider,
        ILogger logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _changeToken = new CancellationTokenSource();
        _config = LoadConfig();
    }

    public IProxyConfig GetConfig() => _config;

    private IProxyConfig LoadConfig()
    {
        _logger.Here().MethodEntered();
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            // First get all active clusters with their destinations
            var clusterIncludes = new Expression<Func<ProxyCluster, object>>[] { c => c.Destinations };
            var clusters = unitOfWork.Clusters.GetAllAsync(clusterIncludes).Result
                .Where(c => c.IsActive)
                .Select(cluster =>
                {
                    var destinations = cluster.Destinations
                        .Where(d => d.IsActive)
                        .ToDictionary(
                            d => d.DestinationId,
                            d => new DestinationConfig { Address = d.Address }
                        );

                    if (!destinations.Any())
                    {
                        _logger.Here().Warning("Cluster {ClusterId} has no active destinations", cluster.ClusterId);
                        return null;
                    }

                    return new ClusterConfig
                    {
                        ClusterId = cluster.ClusterId,
                        LoadBalancingPolicy = cluster.LoadBalancingPolicy,
                        Metadata = cluster.HealthCheckEnabled ? new Dictionary<string, string>
                        {
                            { "HealthCheck:Active:Enabled", "true" },
                            { "HealthCheck:Active:Path", cluster.HealthCheckPath },
                            { "HealthCheck:Active:Interval", cluster.HealthCheckInterval.ToString() },
                            { "HealthCheck:Active:Timeout", cluster.HealthCheckTimeout.ToString() }
                        } : [],
                        Destinations = destinations
                    };
                })
                .Where(c => c != null)
                .ToList();

            _logger.Here().Information("Loaded {ClusterCount} active clusters", clusters.Count);
            foreach (var cluster in clusters)
            {
                _logger.Here().Information("Cluster {ClusterId} has {DestinationCount} active destinations", 
                    cluster.ClusterId, cluster.Destinations.Count);
            }

            // Get all active routes with their cluster, transforms, and headers
            var routeIncludes = new Expression<Func<ProxyRoute, object>>[] { r => r.Cluster, r => r.Transforms, r => r.Headers };
            var routes = unitOfWork.Routes.GetAllAsync(routeIncludes).Result
                .Where(r => r.IsActive && r.Cluster != null && r.Cluster.IsActive)
                .Select(route =>
                {
                    if (route.Cluster == null)
                    {
                        _logger.Here().Warning("Route {RouteId} has no associated cluster", route.RouteId);
                        return null;
                    }

                    // Check if the cluster exists in our active clusters list
                    var matchingCluster = clusters.FirstOrDefault(c => c.ClusterId == route.Cluster.ClusterId);
                    if (matchingCluster == null)
                    {
                        _logger.Here().Warning("Route {RouteId} references cluster {ClusterId} which has no active destinations", 
                            route.RouteId, route.Cluster.ClusterId);
                        return null;
                    }

                    return new RouteConfig
                    {
                        RouteId = route.RouteId,
                        Match = new RouteMatch
                        {
                            Path = route.Path,
                            Methods = route.Methods,
                            Headers = route.Headers?
                                .Where(h => h.IsActive)
                                .Select(h => new RouteHeader
                                {
                                    Name = h.Name,
                                    Values = h.Values,
                                    Mode = ParseHeaderMatchMode(h.Mode),
                                    IsCaseSensitive = true
                                })
                                .ToList()
                        },
                        Transforms = route.Transforms?
                            .Where(t => t.IsActive)
                            .Select(t => new Dictionary<string, string>
                            {
                                { "PathPattern", t.PathPattern }
                            })
                            .ToList() ?? [],
                        Metadata = route.Metadata ?? [],
                        ClusterId = route.Cluster.ClusterId,
                        RateLimiterPolicy = route.RateLimiterPolicy
                    };
                })
                .Where(r => r != null)
                .ToList();

            _logger.Here().Information("Loaded {RouteCount} active routes", routes.Count);
            foreach (var route in routes)
            {
                _logger.Here().Information("Route {RouteId} is mapped to cluster {ClusterId}", 
                    route.RouteId, route.ClusterId);
            }

            var config = new YarpProxyConfig(routes, clusters, _changeToken.Token);
            _logger.Here().MethodExited(config);
            return config;
        }
        catch (Exception ex)
        {
            _logger.Here().Error(ex, "Error loading proxy configuration");
            throw;
        }
    }

    public void Update()
    {
        _logger.Here().MethodEntered();
        try
        {
            var oldToken = _changeToken;
            _changeToken = new CancellationTokenSource();
            _config = LoadConfig();
            oldToken.Cancel();
            _logger.Here().Information("Successfully updated proxy configuration");
            _logger.Here().MethodExited();
        }
        catch (Exception ex)
        {
            _logger.Here().Error(ex, "Error updating proxy configuration");
            throw;
        }
    }

    private static HeaderMatchMode ParseHeaderMatchMode(string mode)
    {
        return mode?.ToLowerInvariant() switch
        {
            "exactheader" => HeaderMatchMode.ExactHeader,
            "headerprefix" => HeaderMatchMode.HeaderPrefix,
            "exists" => HeaderMatchMode.Exists,
            "contains" => HeaderMatchMode.Contains,
            "notcontains" => HeaderMatchMode.NotContains,
            "notexists" => HeaderMatchMode.NotExists,
            _ => HeaderMatchMode.ExactHeader
        };
    }
}

internal class YarpProxyConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters, CancellationToken changeToken) : IProxyConfig
{
    public IReadOnlyList<RouteConfig> Routes { get; } = routes;
    public IReadOnlyList<ClusterConfig> Clusters { get; } = clusters;
    public IChangeToken ChangeToken { get; } = new CancellationChangeToken(changeToken);
} 
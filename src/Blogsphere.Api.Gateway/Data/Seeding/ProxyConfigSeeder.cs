using Blogsphere.Api.Gateway.Entity;
using Blogsphere.Api.Gateway.Extensions;
using Blogsphere.Api.Gateway.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Blogsphere.Api.Gateway.Data.Seeding;

public class ProxyConfigSeeder : IProxyConfigSeeder
{
    private readonly IProxyClusterService _clusterService;
    private readonly IProxyRouteService _routeService;
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;

    public ProxyConfigSeeder(
        IProxyClusterService clusterService,
        IProxyRouteService routeService,
        IConfiguration configuration,
        ILogger logger)
    {
        _clusterService = clusterService;
        _routeService = routeService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        _logger.Here().MethodEntered();
        try
        {
            if (await _clusterService.AnyAsync() || await _routeService.AnyAsync())
            {
                _logger.Here().Information("Database already contains proxy configuration data. Skipping seeding.");
                _logger.Here().MethodExited();
                return;
            }

            var reverseProxyConfig = _configuration.GetSection("ReverseProxy");
            var routes = reverseProxyConfig.GetSection("Routes").GetChildren();
            var clusters = reverseProxyConfig.GetSection("Clusters").GetChildren();

            _logger.Here().Information("Starting to seed clusters from configuration");
            
            // First seed clusters and their destinations
            foreach (var clusterConfig in clusters)
            {
                var clusterId = clusterConfig.Key;
                _logger.Here().Information("Creating cluster {ClusterId}", clusterId);

                var healthCheck = clusterConfig.GetSection("HealthCheck");
                var healthCheckEnabled = healthCheck?.GetValue<bool>("Active:Enabled") ?? true;
                
                var cluster = new ProxyCluster
                {
                    Id = Guid.NewGuid(),
                    ClusterId = clusterId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    LoadBalancingPolicy = clusterConfig.GetValue<string>("LoadBalancingPolicy") ?? "RoundRobin",
                    HealthCheckEnabled = healthCheckEnabled,
                    HealthCheckPath = healthCheck?.GetValue<string>("Active:Path") ?? "/healthcheck",
                    HealthCheckInterval = healthCheck?.GetValue<int>("Active:Interval") ?? 30,
                    HealthCheckTimeout = healthCheck?.GetValue<int>("Active:Timeout") ?? 10
                };

                var destinations = clusterConfig.GetSection("Destinations").GetChildren();
                foreach (var destConfig in destinations)
                {
                    var destinationId = destConfig.Key;
                    var address = destConfig.GetValue<string>("Address");
                    
                    if (string.IsNullOrEmpty(address))
                    {
                        _logger.Here().Warning("Skipping destination {DestinationId} in cluster {ClusterId} due to missing address", 
                            destinationId, clusterId);
                        continue;
                    }

                    _logger.Here().Information("Adding destination {DestinationId} with address {Address} to cluster {ClusterId}", 
                        destinationId, address, clusterId);

                    var destination = new ProxyDestination
                    {
                        Id = Guid.NewGuid(),
                        DestinationId = destinationId,
                        Address = address,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        ClusterId = cluster.Id
                    };
                    cluster.Destinations.Add(destination);
                }

                if (!cluster.Destinations.Any())
                {
                    _logger.Here().Warning("Cluster {ClusterId} has no valid destinations, adding default localhost destination", clusterId);
                    var defaultDestination = new ProxyDestination
                    {
                        Id = Guid.NewGuid(),
                        DestinationId = "default",
                        Address = "http://localhost:8001",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        ClusterId = cluster.Id
                    };
                    cluster.Destinations.Add(defaultDestination);
                }

                await _clusterService.CreateAsync(cluster);
                _logger.Here().Information("Successfully created cluster {ClusterId} with {DestinationCount} destinations", 
                    clusterId, cluster.Destinations.Count);
            }

            _logger.Here().Information("Starting to seed routes from configuration");

            // Then seed routes and their configurations
            foreach (var routeConfig in routes)
            {
                var routeId = routeConfig.Key;
                var clusterId = routeConfig.GetValue<string>("ClusterId");
                
                if (string.IsNullOrEmpty(clusterId))
                {
                    _logger.Here().Warning("Skipping route {RouteId} due to missing ClusterId", routeId);
                    continue;
                }

                _logger.Here().Information("Creating route {RouteId} for cluster {ClusterId}", routeId, clusterId);

                var cluster = await _clusterService.GetByClusterIdAsync(clusterId);

                if (cluster == null)
                {
                    _logger.Here().Warning("Cluster {ClusterId} not found for route {RouteId}, creating default cluster", clusterId, routeId);
                    
                    cluster = new ProxyCluster
                    {
                        Id = Guid.NewGuid(),
                        ClusterId = clusterId,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        LoadBalancingPolicy = "RoundRobin",
                        HealthCheckEnabled = true,
                        HealthCheckPath = "/healthcheck",
                        HealthCheckInterval = 30,
                        HealthCheckTimeout = 10,
                        Destinations =
                        [
                            new()
                            {
                                Id = Guid.NewGuid(),
                                DestinationId = "default",
                                Address = "http://localhost:8001",
                                IsActive = true,
                                CreatedAt = DateTime.UtcNow
                            }
                        ]
                    };
                    cluster.Destinations.First().ClusterId = cluster.Id;
                    await _clusterService.CreateAsync(cluster);
                }

                var route = new ProxyRoute
                {
                    Id = Guid.NewGuid(),
                    RouteId = routeId,
                    ClusterId = cluster.Id,
                    Path = routeConfig.GetValue<string>("Match:Path"),
                    Methods = routeConfig.GetSection("Match:Method").Get<string[]>() ?? new[] { "GET" },
                    RateLimiterPolicy = routeConfig.GetValue<string>("RateLimiterPolicy"),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    Headers = [],
                    Transforms = []
                };

                // Add headers if specified
                var headers = routeConfig.GetSection("Match:Headers").GetChildren();
                foreach (var headerConfig in headers)
                {
                    var headerName = headerConfig.GetValue<string>("Name");
                    if (string.IsNullOrEmpty(headerName))
                    {
                        continue;
                    }

                    _logger.Here().Information("Adding header {HeaderName} to route {RouteId}", headerName, routeId);

                    var header = new ProxyHeader
                    {
                        Id = Guid.NewGuid(),
                        Name = headerName,
                        Values = headerConfig.GetSection("Values").Get<string[]>() ?? Array.Empty<string>(),
                        Mode = headerConfig.GetValue<string>("Mode") ?? "ExactHeader",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        RouteId = route.Id
                    };
                    route.Headers.Add(header);
                }

                // Add transforms if specified
                var transforms = routeConfig.GetSection("Transforms").GetChildren();
                foreach (var transformConfig in transforms)
                {
                    var pathPattern = transformConfig.GetValue<string>("PathPattern");
                    if (string.IsNullOrEmpty(pathPattern))
                    {
                        continue;
                    }

                    _logger.Here().Information("Adding transform with pattern {PathPattern} to route {RouteId}", 
                        pathPattern, routeId);

                    var transform = new ProxyTransform
                    {
                        Id = Guid.NewGuid(),
                        PathPattern = pathPattern,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        RouteId = route.Id
                    };
                    route.Transforms.Add(transform);
                }

                await _routeService.CreateAsync(route);
                _logger.Here().Information("Successfully created route {RouteId} with {HeaderCount} headers and {TransformCount} transforms", 
                    routeId, route.Headers.Count, route.Transforms.Count);
            }

            _logger.Here().Information("Successfully completed seeding proxy configuration data");
            _logger.Here().MethodExited();
        }
        catch (Exception ex)
        {
            _logger.Here().Error(ex, "An error occurred while seeding proxy configuration data");
            throw;
        }
    }
} 
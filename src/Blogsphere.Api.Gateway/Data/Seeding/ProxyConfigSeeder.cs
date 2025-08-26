namespace Blogsphere.Api.Gateway.Data.Seeding;

public class ProxyConfigSeeder(
    IProxyClusterService clusterService,
    IProxyRouteService routeService,
    IConfiguration configuration,
    ILogger logger) : IProxyConfigSeeder
{
    private readonly IProxyClusterService _clusterService = clusterService;
    private readonly IProxyRouteService _routeService = routeService;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger _logger = logger;

    public async Task SeedAsync()
    {
        _logger.Here().MethodEntered();
        try
        {
            var hasCluster = await _clusterService.AnyAsync();
            var hasRoute = await _routeService.AnyAsync();
            
            if (hasCluster.IsSuccess && hasCluster.Value || hasRoute.IsSuccess && hasRoute.Value)
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
                
                var clusterDto = new ProxyClusterDto
                {
                    ClusterId = clusterId,
                    IsActive = true,
                    Metadata = new MetaDataDto
                    {
                        CreatedAt = DateTime.UtcNow
                    },
                    LoadBalancingPolicy = clusterConfig.GetValue<string>("LoadBalancingPolicy") ?? "RoundRobin",
                    HealthCheckEnabled = healthCheckEnabled,
                    HealthCheckPath = healthCheck?.GetValue<string>("Active:Path") ?? "/healthcheck",
                    HealthCheckInterval = healthCheck?.GetValue<int>("Active:Interval") ?? 30,
                    HealthCheckTimeout = healthCheck?.GetValue<int>("Active:Timeout") ?? 10,
                    Destinations = []
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

                    var destinationDto = new ProxyDestinationDto
                    {
                        Id = Guid.NewGuid(),
                        DestinationId = destinationId,
                        Address = address,
                        IsActive = true,
                        ClusterId = clusterDto.Id
                    };
                    clusterDto.Destinations.Add(destinationDto);
                }

                if (!clusterDto.Destinations.Any())
                {
                    _logger.Here().Warning("Cluster {ClusterId} has no valid destinations, adding default localhost destination", clusterId);
                    var defaultDestinationDto = new ProxyDestinationDto
                    {
                        Id = Guid.NewGuid(),
                        DestinationId = "default",
                        Address = "http://localhost:8001",
                        IsActive = true,
                        ClusterId = clusterDto.Id
                    };
                    clusterDto.Destinations.Add(defaultDestinationDto);
                }

                var createdCluster = await _clusterService.CreateAsync(clusterDto, null);
                if (!createdCluster.IsSuccess)
                {
                    _logger.Here().Error("Failed to create cluster {ClusterId}: {ErrorMessage}", 
                        clusterId, createdCluster.ErrorMessage);
                    continue;
                }

                _logger.Here().Information("Successfully created cluster {ClusterId} with {DestinationCount} destinations", 
                    clusterId, clusterDto.Destinations.Count);
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

                var clusterResult = await _clusterService.GetByClusterIdAsync(clusterId);
                var cluster = clusterResult.IsSuccess ? clusterResult.Value : null;

                if (cluster == null)
                {
                    _logger.Here().Warning("Cluster {ClusterId} not found for route {RouteId}, creating default cluster", clusterId, routeId);
                    
                    var defaultClusterDto = new ProxyClusterDto
                    {
                        Id = Guid.NewGuid(),
                        ClusterId = clusterId,
                        IsActive = true,
                        Metadata = new MetaDataDto
                        {
                            CreatedAt = DateTime.UtcNow
                        },
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
                            }
                        ]
                    };
                    defaultClusterDto.Destinations.First().ClusterId = defaultClusterDto.Id;
                    var defaultClusterResult = await _clusterService.CreateAsync(defaultClusterDto, null);
                    if (!defaultClusterResult.IsSuccess)
                    {
                        _logger.Here().Error("Failed to create default cluster {ClusterId}: {ErrorMessage}", 
                            clusterId, defaultClusterResult.ErrorMessage);
                        continue;
                    }
                    cluster = defaultClusterResult.Value;
                }

                var routeDto = new ProxyRouteDto
                {
                    Id = Guid.NewGuid(),
                    RouteId = routeId,
                    ClusterId = cluster.Id,
                    Path = routeConfig.GetValue<string>("Match:Path"),
                    Methods = routeConfig.GetSection("Match:Method").Get<string[]>() ?? ["GET"],
                    RateLimiterPolicy = routeConfig.GetValue<string>("RateLimiterPolicy"),
                    IsActive = true,
                    Metadata = new MetaDataDto
                    {
                        CreatedAt = DateTime.UtcNow
                    },
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

                    var header = new ProxyHeaderDto
                    {
                        Id = Guid.NewGuid(),
                        Name = headerName,
                        Values = headerConfig.GetSection("Values").Get<string[]>() ?? Array.Empty<string>(),
                        Mode = headerConfig.GetValue<string>("Mode") ?? "ExactHeader"
                    };
                    routeDto.Headers.Add(header);
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

                    var transform = new ProxyTransformDto
                    {
                        Id = Guid.NewGuid(),
                        PathPattern = pathPattern
                    };
                    routeDto.Transforms.Add(transform);
                }

                var createdRoute = await _routeService.CreateAsync(routeDto, null);
                if (!createdRoute.IsSuccess)
                {
                    _logger.Here().Error("Failed to create route {RouteId}: {ErrorMessage}", 
                        routeId, createdRoute.ErrorMessage);
                    continue;
                }

                _logger.Here().Information("Successfully created route {RouteId} with {HeaderCount} headers and {TransformCount} transforms", 
                    routeId, routeDto.Headers.Count, routeDto.Transforms.Count);
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
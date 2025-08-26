using Swashbuckle.AspNetCore.Filters;

namespace Blogsphere.Api.Gateway.Swagger.Examples;

public class PaginatedProxyClusterResponseExample : IExamplesProvider<PaginatedResult<ProxyClusterDto>>
{
    public PaginatedResult<ProxyClusterDto> GetExamples()
    {
        return new PaginatedResult<ProxyClusterDto>
        {
            Items = [
                new ProxyClusterDto
                {
                    Id = Guid.NewGuid(),
                    ClusterId = "blog-cluster-1",
                    LoadBalancingPolicy = "RoundRobin",
                    HealthCheckEnabled = true,
                    HealthCheckPath = "/healthcheck",
                    HealthCheckInterval = 30,
                    HealthCheckTimeout = 10,
                    IsActive = true,
                    Metadata = new MetaDataDto
                    {
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        CreatedBy = "test",
                        UpdatedBy = "test"
                    },
                    Destinations = [
                        new ProxyDestinationDto
                        {
                            Id = Guid.NewGuid(),
                            DestinationId = "blog-api-1",
                            Address = "http://blog-api-1:8080",
                            IsActive = true,
                        }
                    ]
                },
                new ProxyClusterDto
                {
                    Id = Guid.NewGuid(),
                    ClusterId = "blog-cluster-2",
                    LoadBalancingPolicy = "LeastRequests",
                    HealthCheckEnabled = true,
                    HealthCheckPath = "/healthcheck",
                    HealthCheckInterval = 30,
                    HealthCheckTimeout = 10,
                    IsActive = true,
                    Metadata = new MetaDataDto
                    {
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        CreatedBy = "test",
                        UpdatedBy = "test"
                    },
                    Destinations = [
                        new ProxyDestinationDto
                        {
                            Id = Guid.NewGuid(),
                            DestinationId = "blog-api-2",
                            Address = "http://blog-api-2:8080",
                            IsActive = true,
                        }
                    ]
                }
            ],
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 2
        };
    }
} 
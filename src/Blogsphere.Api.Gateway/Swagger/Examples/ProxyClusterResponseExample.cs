using Swashbuckle.AspNetCore.Filters;

namespace Blogsphere.Api.Gateway.Swagger.Examples;

public class ProxyClusterResponseExample : IExamplesProvider<ProxyClusterDto>
{
    public ProxyClusterDto GetExamples()
    {
        return new ProxyClusterDto
        {
            Id = Guid.NewGuid(),
            ClusterId = "backend-services",
            LoadBalancingPolicy = "RoundRobin",
            IsActive = true,
            Metadata = new MetaDataDto
            {
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = "test",
                UpdatedBy = "test"
            },
            Destinations =
            [
                new()
                {
                    Id = Guid.NewGuid(),
                    DestinationId = "backend-1",
                    Address = "https://backend1.example.com",
                    IsActive = true
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    DestinationId = "backend-2",
                    Address = "https://backend2.example.com",
                    IsActive = true
                }
            ]
        };
    }
} 
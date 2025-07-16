using Blogsphere.Api.Gateway.Models.DTOs;
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
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Destinations = new List<ProxyDestinationDto>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    DestinationId = "backend-1",
                    Address = "https://backend1.example.com",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    DestinationId = "backend-2",
                    Address = "https://backend2.example.com",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            }
        };
    }
} 
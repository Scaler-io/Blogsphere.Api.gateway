using Swashbuckle.AspNetCore.Filters;

namespace Blogsphere.Api.Gateway.Swagger.Examples;

public class ProxyClusterSearchableResponseExample : IExamplesProvider<IEnumerable<ProxyClusterSearchableDto>>
{
    public IEnumerable<ProxyClusterSearchableDto> GetExamples()
    {
        return
        [
            new() {
                Id = Guid.NewGuid().ToString(),
                LoadBalancerName = "RoundRobin",
                DestinationCount = 1,
                RouteCount = 1,
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
            new() {
                Id = Guid.NewGuid().ToString(),
                LoadBalancerName = "RoundRobin",
                DestinationCount = 1,
                RouteCount = 1,
                Status = "Inactive",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            }
        ];
    }
}

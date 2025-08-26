using Swashbuckle.AspNetCore.Filters;

namespace Blogsphere.Api.Gateway.Swagger.Examples;

public class ProxyRouteResponseExample : IExamplesProvider<ProxyRouteDto>
{
    public ProxyRouteDto GetExamples()
    {
        return new ProxyRouteDto
        {
            Id = Guid.NewGuid(),
            RouteId = "blog-api-route",
            Path = "/api/blog/{**catch-all}",
            Methods = ["GET", "POST", "PUT", "DELETE"],
            RateLimiterPolicy = "default",
            Metadata = new MetaDataDto
            {
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = "test",
                UpdatedBy = "test"
            },
            IsActive = true,
            ClusterId = Guid.NewGuid(),
            Headers = [
                new ProxyHeaderDto
                {
                    Id = Guid.NewGuid(),
                    Name = "X-Api-Version",
                    Values = ["1.0"],
                    Mode = "Append"
                }
            ],
            Transforms = [
                new ProxyTransformDto
                {
                    Id = Guid.NewGuid(),
                    PathPattern = "api/blog/{**catch-all}"
                }
            ]
        };
    }
} 
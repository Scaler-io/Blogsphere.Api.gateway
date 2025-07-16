using Blogsphere.Api.Gateway.Models.DTOs;
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
            Metadata = new Dictionary<string, string>
            {
                { "Version", "1.0" },
                { "Environment", "Production" }
            },
            IsActive = true,
            ClusterId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
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
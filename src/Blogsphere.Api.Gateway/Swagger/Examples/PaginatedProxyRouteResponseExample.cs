using Blogsphere.Api.Gateway.Models.Common;
using Blogsphere.Api.Gateway.Models.DTOs;
using Swashbuckle.AspNetCore.Filters;

namespace Blogsphere.Api.Gateway.Swagger.Examples;

public class PaginatedProxyRouteResponseExample : IExamplesProvider<PaginatedResult<ProxyRouteDto>>
{
    public PaginatedResult<ProxyRouteDto> GetExamples()
    {
        return new PaginatedResult<ProxyRouteDto>
        {
            Items = [
                new ProxyRouteDto
                {
                    Id = Guid.NewGuid(),
                    RouteId = "blog-api-route-1",
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
                },
                new ProxyRouteDto
                {
                    Id = Guid.NewGuid(),
                    RouteId = "blog-api-route-2",
                    Path = "/api/blog/v2/{**catch-all}",
                    Methods = ["GET", "POST", "PUT", "DELETE"],
                    RateLimiterPolicy = "default",
                    Metadata = new Dictionary<string, string>
                    {
                        { "Version", "2.0" },
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
                            Values = ["2.0"],
                            Mode = "Append"
                        }
                    ],
                    Transforms = [
                        new ProxyTransformDto
                        {
                            Id = Guid.NewGuid(),
                            PathPattern = "api/blog/v2/{**catch-all}"
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
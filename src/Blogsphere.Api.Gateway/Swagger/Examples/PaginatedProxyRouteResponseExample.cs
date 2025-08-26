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
                },
                new ProxyRouteDto
                {
                    Id = Guid.NewGuid(),
                    RouteId = "blog-api-route-2",
                    Path = "/api/blog/v2/{**catch-all}",
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
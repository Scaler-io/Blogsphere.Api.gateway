using Blogsphere.Api.Gateway.Models.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace Blogsphere.Api.Gateway.Swagger.Examples;

public class CreateProxyRouteRequestExample : IExamplesProvider<CreateProxyRouteRequest>
{
    public CreateProxyRouteRequest GetExamples()
    {
        return new CreateProxyRouteRequest
        {
            RouteId = "blog-api-route",
            Path = "/api/blog/{**catch-all}",
            Methods = ["GET", "POST", "PUT", "DELETE"],
            RateLimiterPolicy = "default",
            Metadata = new Dictionary<string, string>
            {
                { "Version", "1.0" },
                { "Environment", "Production" }
            },
            ClusterId = Guid.NewGuid(),
            Headers = [
                new ProxyHeaderRequest
                {
                    Name = "X-Api-Version",
                    Values = ["1.0"],
                    Mode = "Append"
                }
            ],
            Transforms = [
                new ProxyTransformRequest
                {
                    PathPattern = "api/blog/{**catch-all}"
                }
            ]
        };
    }
} 
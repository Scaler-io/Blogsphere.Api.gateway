using Blogsphere.Api.Gateway.Models.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace Blogsphere.Api.Gateway.Swagger.Examples;

public class CreateProxyClusterRequestExample : IExamplesProvider<CreateProxyClusterRequest>
{
    public CreateProxyClusterRequest GetExamples()
    {
        return new CreateProxyClusterRequest
        {
            ClusterId = "backend-services",
            LoadBalancingPolicy = "RoundRobin",
            Destinations = new List<CreateProxyDestinationRequest>
            {
                new()
                {
                    DestinationId = "backend-1",
                    Address = "https://backend1.example.com"
                },
                new()
                {
                    DestinationId = "backend-2",
                    Address = "https://backend2.example.com"
                }
            }
        };
    }
} 
using Swashbuckle.AspNetCore.Filters;

namespace Blogsphere.Api.Gateway.Swagger.Examples;

public class CreateSubscribedApiRequestExample : IExamplesProvider<CreateSubscribedApiRequest>
{   
    public CreateSubscribedApiRequest GetExamples()
    {
        return new CreateSubscribedApiRequest
        {
            ApiName = "TestApi",
            ApiPath = "/test",
            ApiDescription = "Test Api Description",
            ProductId = Guid.NewGuid()
        };
    }
}

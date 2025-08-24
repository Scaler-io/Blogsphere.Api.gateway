using Swashbuckle.AspNetCore.Filters;

namespace Blogsphere.Api.Gateway.Swagger.Examples;

public class CreateApiProductRequestExample : IExamplesProvider<CreateApiProductRequest>
{
    public CreateApiProductRequest GetExamples()
    {
        return new CreateApiProductRequest
        {
            ProductName = "Test API Product",
            ProductDescription = "This is a test api product",
        };
    }
}

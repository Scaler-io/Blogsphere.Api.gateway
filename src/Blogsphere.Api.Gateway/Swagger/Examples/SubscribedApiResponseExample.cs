using Swashbuckle.AspNetCore.Filters;

namespace Blogsphere.Api.Gateway.Swagger.Examples;

public class SubscribedApiResponseExample : IExamplesProvider<SubscribedApiDto>
{
    public SubscribedApiDto GetExamples()
    {
        return new SubscribedApiDto
        {
            ApiName = "TestApi",
            ApiPath = "/test",
            ApiDescription = "Test Api Description",
            ApiProductDetails = new ApiProductSummary
            {
                Id = Guid.NewGuid(),
                ProductName = "TestProduct"
            },
            CreatedBy = "TestUser",
            CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
            UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
        };
    }
}

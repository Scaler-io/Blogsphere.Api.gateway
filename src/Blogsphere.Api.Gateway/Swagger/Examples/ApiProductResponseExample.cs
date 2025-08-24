using Swashbuckle.AspNetCore.Filters;

namespace Blogsphere.Api.Gateway.Swagger.Examples;

public class ApiProductResponseExample : IExamplesProvider<ApiProductDto>
{
    public ApiProductDto GetExamples()
    {
        return new ApiProductDto
        {
            ProductId = Guid.NewGuid().ToString(),
            ProductName = "Test API Product",
            ProductDescription = "This is a test api product",
            IsActive = true,
            CreatedAt = DateTime.UtcNow.ToString(),
            UpdatedAt = DateTime.UtcNow.ToString(),
            CreatedBy = "Test User",
            UpdatedBy = "Test User",
        };
    }
}

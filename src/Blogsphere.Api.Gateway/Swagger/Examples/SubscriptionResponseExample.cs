using Swashbuckle.AspNetCore.Filters;

namespace Blogsphere.Api.Gateway.Swagger.Examples;

public class SubscriptionResponseExample : IExamplesProvider<SubscriptionDto>
{
    public SubscriptionDto GetExamples()
    {
        return new SubscriptionDto
        {
            Id = Guid.NewGuid(),
            SubscriptionName = "Test Subscription",
            SubscriptionDescription = "Test Subscription Description",
            ApiProductDetails = new ApiProductSummary
            {
                Id = Guid.NewGuid(),
                ProductName = "Test Product"
            },
            CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss tt"),
            UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss tt"),
            CreatedBy = "Test User",
            UpdatedBy = "Test User" 
        };
    }
}

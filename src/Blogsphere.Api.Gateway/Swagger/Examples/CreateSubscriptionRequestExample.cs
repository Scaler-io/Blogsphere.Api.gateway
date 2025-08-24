using Swashbuckle.AspNetCore.Filters;

namespace Blogsphere.Api.Gateway.Swagger.Examples;

public class CreateSubscriptionRequestExample : IExamplesProvider<CreateSubscriptionRequest>    
{
    public CreateSubscriptionRequest GetExamples()
    {
        return new CreateSubscriptionRequest
        {
            SubscriptionName = "Test Subscription",
            SubscriptionDescription = "Test Subscription Description"
        };
    }
}

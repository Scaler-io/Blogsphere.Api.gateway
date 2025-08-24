namespace Blogsphere.Api.Gateway.Models.Requests.Subscription;

public class CreateSubscriptionRequest
{
    public string SubscriptionName { get; set; }
    public string SubscriptionDescription { get; set; }
    public Guid ProductId { get; set; }
}

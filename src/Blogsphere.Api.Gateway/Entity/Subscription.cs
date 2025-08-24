namespace Blogsphere.Api.Gateway.Entity;

public class Subscription : EntityBase
{
    public string SubscriptionKey { get; set; } = Guid.NewGuid().ToString();
    public string SubscriptionName { get; set; }
    public string SubscriptionDescription { get; set; }
    public Guid ProductId { get; set; }
    public ApiProduct ApiProduct { get; set; }
}

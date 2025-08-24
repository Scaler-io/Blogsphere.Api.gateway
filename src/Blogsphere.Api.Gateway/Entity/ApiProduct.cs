namespace Blogsphere.Api.Gateway.Entity;

public class ApiProduct : EntityBase
{
    public string ProductName { get; set; }
    public string ProductDescription { get; set; }
    public ICollection<SubscribedApi> SubscribedApis { get; set; }
    public ICollection<Subscription> Subscriptions { get; set; }
}

namespace Blogsphere.Api.Gateway.Services;

public interface ISubscriptionService
{
    Task<bool> CheckSubscriptionValidity(string subscriptionKey, string apiPath);
}

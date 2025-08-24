namespace Blogsphere.Api.Gateway.Services.Interfaces;

public interface ISubscriptionRetrievaService
{
    Task<Result<List<string>>> GetSubscribedApiPathsAsync(string subscriptionKey);
}

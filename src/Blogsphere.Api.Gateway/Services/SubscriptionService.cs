namespace Blogsphere.Api.Gateway.Services;

public class SubscriptionService(ILogger logger, ISubscriptionRetrievaService subscriptionRetrievaService) : ISubscriptionService
{
    private readonly ILogger _logger = logger;
    private readonly ISubscriptionRetrievaService _subscriptionRetrievaService = subscriptionRetrievaService;

    public Task<bool> CheckSubscriptionValidity(string subscriptionKey, string apiPath)
    {
        _logger.Here().MethodEntered();
        
        var apiPaths =  _subscriptionRetrievaService.GetSubscribedApiPathsAsync(subscriptionKey).Result;
        if(!apiPaths.IsSuccess)
        {
            _logger.Here().Error("No subscribed api found");
            _logger.Here().MethodExited();
            return Task.FromResult(false);
        }

        return Task.FromResult(apiPaths.Value.Contains(apiPath));
    }
}

using Blogsphere.Api.Gateway.Extensions;

namespace Blogsphere.Api.Gateway.Services;

public class SubscriptionService(ILogger logger) : ISubscriptionService
{
    private readonly ILogger _logger = logger;

    private readonly Dictionary<string, List<string>> _subscriptions = new()
    {
        { "7B6AD94DCC3C4E9F891C52C8C340D99E", [ "/user" ] }
    };

    public Task<bool> CheckSubscriptionValidity(string subscriptionKey, string apiPath)
    {
        _logger.Here().MethodEntered();
        
        if (_subscriptions.TryGetValue(subscriptionKey, out var subscribedApis))
        {
            return Task.FromResult(subscribedApis.Contains(apiPath));
        }

        _logger.Here().Error("No subscribed api found");
        _logger.Here().MethodExited();
        return Task.FromResult(false);
    }
}

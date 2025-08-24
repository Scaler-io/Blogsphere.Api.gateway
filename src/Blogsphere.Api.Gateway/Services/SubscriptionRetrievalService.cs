namespace Blogsphere.Api.Gateway.Services;

public class SubscriptionRetrievalService(
    IApiProductManageService apiProductManageService,
    ISubscriptionManageService subscriptionManageService,
    ISubscribedApiManageService subscribedApiManageService) : ISubscriptionRetrievaService
{
    private readonly IApiProductManageService _apiProductManageService = apiProductManageService;
    private readonly ISubscriptionManageService _subscriptionManageService = subscriptionManageService;
    private readonly ISubscribedApiManageService _subscribedApiManageService = subscribedApiManageService;

    public async Task<Result<List<string>>> GetSubscribedApiPathsAsync(string subscriptionKey)
    {
        var subscription = await _subscriptionManageService.GetSubscriptionByKeyAsync(subscriptionKey);
        if(!subscription.IsSuccess)
        {
            return Result<List<string>>.Failure(ErrorCodes.NotFound);
        }

        var apiProduct = await _apiProductManageService.GetApiProductByIdAsync(subscription.Value.ApiProductDetails.Id);
        
        Guid.TryParse(apiProduct.Value.ProductId, out Guid productId);
        var subscribedApis = await _subscribedApiManageService.GetSubscribedApisByProductIdAsync(productId);

        return Result<List<string>>.Success([.. subscribedApis.Value.Select(x => x.ApiPath)]);
    }
}

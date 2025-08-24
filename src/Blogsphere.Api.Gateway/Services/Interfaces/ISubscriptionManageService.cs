namespace Blogsphere.Api.Gateway.Services.Interfaces;

public interface ISubscriptionManageService
{
    Task<Result<PaginatedResult<SubscriptionDto>>> GetAllSubscriptionsAsync(PaginationRequest request);
    Task<Result<SubscriptionDto>> GetSubscriptionByIdAsync(Guid id);
    Task<Result<SubscriptionDto>> CreateSubscriptionAsync(CreateSubscriptionRequest request, RequestInformation requestInformation);
    Task<Result<bool>> DeleteSubscriptionAsync(Guid id);
    Task<Result<SubscriptionDto>> GetSubscriptionByKeyAsync(string subscriptionKey);
}

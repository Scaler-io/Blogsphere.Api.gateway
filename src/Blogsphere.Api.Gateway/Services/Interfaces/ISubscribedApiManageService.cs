namespace Blogsphere.Api.Gateway.Services.Interfaces;

public interface ISubscribedApiManageService
{
    Task<Result<PaginatedResult<SubscribedApiDto>>> GetAllSubscribedApisAsync(PaginationRequest request);
    
    Task<Result<SubscribedApiDto>> GetSubscribedApiByIdAsync(Guid id);
    Task<Result<SubscribedApiDto>> CreateSubscribedApiAsync(CreateSubscribedApiRequest request, RequestInformation requestInformation);
    Task<Result<bool>> UpdateSubscribedApiAsync(UpdateSubscribedApiRequest request, RequestInformation requestInformation);
    Task<Result<bool>> DeleteSubscribedApiAsync(Guid id);    
    Task<Result<List<SubscribedApiDto>>> GetSubscribedApisByProductIdAsync(Guid productId);
}

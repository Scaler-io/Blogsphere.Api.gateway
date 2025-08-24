namespace Blogsphere.Api.Gateway.Services.Interfaces;

public interface IApiProductManageService
{
    Task<Result<PaginatedResult<ApiProductDto>>> GetAllApiProductsAsync(PaginationRequest request);
    Task<Result<ApiProductDto>> GetApiProductByIdAsync(Guid id);
    Task<Result<ApiProductDto>> CreateApiProductAsync(CreateApiProductRequest request, RequestInformation requestInformation);
    Task<Result<bool>> DeleteApiProductAsync(Guid id, RequestInformation requestInformation);
}

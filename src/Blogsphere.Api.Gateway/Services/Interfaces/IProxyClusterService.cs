namespace Blogsphere.Api.Gateway.Services.Interfaces;

public interface IProxyClusterService
{
    Task<Result<PaginatedResult<ProxyClusterSearchableDto>>> GetAllAsync(PaginationRequest request);
    Task<Result<ProxyClusterDto>> GetByIdAsync(Guid id);
    Task<Result<ProxyClusterDto>> CreateFromRequestAsync(CreateProxyClusterRequest request, RequestInformation requestInfo);
    Task<Result<ProxyClusterDto>> CreateAsync(ProxyClusterDto dto, RequestInformation requestInfo);
    Task<Result<ProxyClusterDto>> UpdateAsync(Guid id, ProxyClusterDto dto, RequestInformation requestInfo);
    Task<Result<ProxyClusterDto>> UpdateFromRequestAsync(Guid id, UpdateProxyClusterRequest request, RequestInformation requestInfo);
    Task<Result<bool>> DeleteAsync(Guid id, RequestInformation requestInfo);
    Task<Result<bool>> AnyAsync();
    Task<Result<ProxyClusterDto>> GetByClusterIdAsync(string clusterId);
}

using Blogsphere.Api.Gateway.Models.Common;
using Blogsphere.Api.Gateway.Models.DTOs;
using Blogsphere.Api.Gateway.Models.Requests;

namespace Blogsphere.Api.Gateway.Services.Interfaces;

public interface IProxyClusterService
{
    Task<Result<PaginatedResult<ProxyClusterDto>>> GetAllAsync(PaginationRequest request);
    Task<Result<ProxyClusterDto>> GetByIdAsync(Guid id);
    Task<Result<ProxyClusterDto>> CreateFromRequestAsync(CreateProxyClusterRequest request);
    Task<Result<ProxyClusterDto>> CreateAsync(ProxyClusterDto dto);
    Task<Result<ProxyClusterDto>> UpdateAsync(Guid id, ProxyClusterDto dto);
    Task<Result<ProxyClusterDto>> UpdateFromRequestAsync(Guid id, UpdateProxyClusterRequest request);
    Task<Result<bool>> DeleteAsync(Guid id);
    Task<Result<bool>> AnyAsync();
    Task<Result<ProxyClusterDto>> GetByClusterIdAsync(string clusterId);
} 
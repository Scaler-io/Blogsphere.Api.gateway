using Blogsphere.Api.Gateway.Models.Common;
using Blogsphere.Api.Gateway.Models.DTOs;
using Blogsphere.Api.Gateway.Models.Requests;

namespace Blogsphere.Api.Gateway.Services.Interfaces;

public interface IProxyRouteService
{
    Task<Result<PaginatedResult<ProxyRouteDto>>> GetAllAsync(PaginationRequest request);
    Task<Result<ProxyRouteDto>> GetByIdAsync(Guid id);
    Task<Result<ProxyRouteDto>> CreateFromRequestAsync(CreateProxyRouteRequest request);
    Task<Result<ProxyRouteDto>> CreateAsync(ProxyRouteDto dto);
    Task<Result<ProxyRouteDto>> UpdateAsync(Guid id, ProxyRouteDto dto);
    Task<Result<ProxyRouteDto>> UpdateFromRequestAsync(Guid id, UpdateProxyRouteRequest request);
    Task<Result<bool>> DeleteAsync(Guid id);
    Task<Result<bool>> AnyAsync();
} 
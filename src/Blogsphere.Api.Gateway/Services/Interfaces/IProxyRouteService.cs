using Blogsphere.Api.Gateway.Models.Common;
using Blogsphere.Api.Gateway.Models.DTOs;
using Blogsphere.Api.Gateway.Models.DTOs.Search;
using Blogsphere.Api.Gateway.Models.Requests;

namespace Blogsphere.Api.Gateway.Services.Interfaces;

public interface IProxyRouteService
{
    Task<Result<PaginatedResult<ProxyRouteSearchableDto>>> GetAllAsync(PaginationRequest request);
    Task<Result<ProxyRouteDto>> GetByIdAsync(Guid id);
    Task<Result<ProxyRouteDto>> CreateFromRequestAsync(CreateProxyRouteRequest request, RequestInformation requestInfo);
    Task<Result<ProxyRouteDto>> CreateAsync(ProxyRouteDto dto, RequestInformation requestInfo);
    Task<Result<ProxyRouteDto>> UpdateAsync(Guid id, ProxyRouteDto dto, RequestInformation requestInfo);
    Task<Result<ProxyRouteDto>> UpdateFromRequestAsync(Guid id, UpdateProxyRouteRequest request, RequestInformation requestInfo);
    Task<Result<bool>> DeleteAsync(Guid id, RequestInformation requestInfo);
    Task<Result<bool>> AnyAsync();
} 
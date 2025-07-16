using Blogsphere.Api.Gateway.Models.Common;
using Blogsphere.Api.Gateway.Models.DTOs;

namespace Blogsphere.Api.Gateway.Services.Interfaces;

public interface IProxyDestinationService
{
    Task<Result<PaginatedResult<ProxyDestinationDto>>> GetAllAsync(PaginationRequest request);
    Task<Result<ProxyDestinationDto>> GetByIdAsync(Guid id);
    Task<Result<ProxyDestinationDto>> CreateAsync(ProxyDestinationDto dto);
    Task<Result<ProxyDestinationDto>> UpdateAsync(Guid id, ProxyDestinationDto dto);
    Task<Result<bool>> DeleteAsync(Guid id);
} 
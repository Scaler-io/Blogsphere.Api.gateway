using Blogsphere.Api.Gateway.Entity;
using Blogsphere.Api.Gateway.Models.Common;

namespace Blogsphere.Api.Gateway.Services.Interfaces.Base;

public interface IBaseService<T> where T : EntityBase
{
    Task<Result<T>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PaginatedResult<T>>> GetAllAsync(PaginationRequest request, CancellationToken cancellationToken = default);
    Task<Result<T>> CreateAsync(T entity, CancellationToken cancellationToken = default);
    Task<Result<T>> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
} 
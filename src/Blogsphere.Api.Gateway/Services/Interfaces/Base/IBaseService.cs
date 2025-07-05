using Blogsphere.Api.Gateway.Entity;
using Blogsphere.Api.Gateway.Infrastructure.Common;

namespace Blogsphere.Api.Gateway.Services.Interfaces.Base;

public interface IBaseService<T> where T : EntityBase
{
    Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaginatedResult<T>> GetAllAsync(PaginationRequest request, CancellationToken cancellationToken = default);
    Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
} 
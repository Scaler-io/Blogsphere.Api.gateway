using System.Linq.Expressions;
using Blogsphere.Api.Gateway.Entity;

namespace Blogsphere.Api.Gateway.Data.Interfaces.Base;

public interface IRepository<TEntity> where TEntity : EntityBase
{
    Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TEntity> GetByIdAsync(Guid id, Expression<Func<TEntity, object>>[] includes, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, object>>[] includes, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>>[] includes, CancellationToken cancellationToken = default);
    
    Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>>[] includes, CancellationToken cancellationToken = default);
    
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void UpdateRange(IEnumerable<TEntity> entities);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
    
    IQueryable<TEntity> AsQueryable();
    IQueryable<TEntity> Include(params Expression<Func<TEntity, object>>[] includes);
} 
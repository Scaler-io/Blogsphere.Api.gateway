using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Blogsphere.Api.Gateway.Data.Repositories.Base;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : EntityBase
{
    protected readonly DbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    protected Repository(DbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public async Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], cancellationToken);
    }

    public async Task<TEntity> GetByIdAsync(Guid id, Expression<Func<TEntity, object>>[] includes, CancellationToken cancellationToken = default)
    {
        var query = ApplyIncludes(includes);
        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, object>>[] includes, CancellationToken cancellationToken = default)
    {
        var query = ApplyIncludes(includes);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>>[] includes, CancellationToken cancellationToken = default)
    {
        var query = ApplyIncludes(includes);
        return await query.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>>[] includes, CancellationToken cancellationToken = default)
    {
        var query = ApplyIncludes(includes);
        return await query.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(predicate, cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await DbSet.AddRangeAsync(entities, cancellationToken);
    }

    public void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        DbSet.UpdateRange(entities);
    }

    public void Remove(TEntity entity)
    {
        DbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        DbSet.RemoveRange(entities);
    }

    public IQueryable<TEntity> AsQueryable()
    {
        return DbSet.AsQueryable();
    }

    public IQueryable<TEntity> Include(params Expression<Func<TEntity, object>>[] includes)
    {
        return ApplyIncludes(includes);
    }

    private IQueryable<TEntity> ApplyIncludes(params Expression<Func<TEntity, object>>[] includes)
    {
        if (includes == null || !includes.Any())
            return DbSet;
            
        return includes.Aggregate(DbSet.AsQueryable(),
            (current, include) => current.Include(include));
    }
} 
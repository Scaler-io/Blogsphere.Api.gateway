using Blogsphere.Api.Gateway.Data.Interfaces;
using Blogsphere.Api.Gateway.Data.Interfaces.Base;
using Blogsphere.Api.Gateway.Entity;
using Blogsphere.Api.Gateway.Extensions;
using Blogsphere.Api.Gateway.Infrastructure.Common;
using Blogsphere.Api.Gateway.Services.Interfaces.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Blogsphere.Api.Gateway.Services.Base;

public abstract class BaseService<T>(
    ILogger logger,
    IRepository<T> repository,
    IUnitOfWork unitOfWork) : IBaseService<T> where T : EntityBase
{
    protected readonly ILogger _logger = logger.ForContext<BaseService<T>>();
    protected readonly IRepository<T> _repository = repository;
    protected readonly IUnitOfWork _unitOfWork = unitOfWork;

    public virtual async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.Here().MethodEntered();
        
        try
        {
            var entity = await _repository.GetByIdAsync(id, GetDefaultIncludes(), cancellationToken);
            
            if (entity == null)
            {
                _logger.Warning("Entity with ID {Id} not found", id);
                throw new InvalidOperationException($"Entity with ID {id} not found");
            }
            
            _logger.Here().MethodExited(entity);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting entity by ID {Id}", id);
            throw;
        }
    }

    public virtual async Task<PaginatedResult<T>> GetAllAsync(PaginationRequest request, CancellationToken cancellationToken = default)
    {
        _logger.Here().MethodEntered();
        
        try
        {
            var query = _repository.Include(GetDefaultIncludes());

            // Apply search if provided
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = ApplySearch(query, request.SearchTerm);
            }

            // Apply sorting if provided
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                query = ApplySort(query, request.SortBy, request.IsDescending);
            }

            var totalCount = await query.CountAsync(cancellationToken);
            
            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var result = new PaginatedResult<T>(items, request.PageNumber, request.PageSize, totalCount);
            _logger.Here().MethodExited(result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting all entities with pagination");
            throw;
        }
    }

    public virtual async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _logger.Here().MethodEntered();
        
        try
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            
            await _repository.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.Information("Created entity with ID {Id}", entity.Id);
            _logger.Here().MethodExited(entity);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error creating entity");
            throw;
        }
    }

    public virtual async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _logger.Here().MethodEntered();
        
        try
        {
            var existingEntity = await _repository.GetByIdAsync(entity.Id, cancellationToken);
            
            if (existingEntity == null)
            {
                _logger.Error("Entity with ID {Id} not found for update", entity.Id);
                throw new InvalidOperationException($"Entity with ID {entity.Id} not found");
            }

            entity.CreatedAt = existingEntity.CreatedAt;
            entity.UpdatedAt = DateTime.UtcNow;
            
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.Information("Updated entity with ID {Id}", entity.Id);
            _logger.Here().MethodExited(entity);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error updating entity with ID {Id}", entity.Id);
            throw;
        }
    }

    public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.Here().MethodEntered();
        
        try
        {
            var entity = await _repository.GetByIdAsync(id, cancellationToken);
            
            if (entity == null)
            {
                _logger.Error("Entity with ID {Id} not found for deletion", id);
                throw new InvalidOperationException($"Entity with ID {id} not found");
            }

            _repository.Remove(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.Information("Deleted entity with ID {Id}", id);
            _logger.Here().MethodExited();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error deleting entity with ID {Id}", id);
            throw;
        }
    }

    protected virtual IQueryable<T> ApplySearch(IQueryable<T> query, string searchTerm)
    {
        // Base implementation returns unmodified query
        // Override in derived classes to implement specific search logic
        return query;
    }

    protected virtual IQueryable<T> ApplySort(IQueryable<T> query, string sortBy, bool isDescending)
    {
        // Base implementation returns unmodified query
        // Override in derived classes to implement specific sorting logic
        return query;
    }

    protected virtual Expression<Func<T, object>>[] GetDefaultIncludes()
    {
        return Array.Empty<Expression<Func<T, object>>>();
    }
} 
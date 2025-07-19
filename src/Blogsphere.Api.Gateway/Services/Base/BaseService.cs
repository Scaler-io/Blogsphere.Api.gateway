using Blogsphere.Api.Gateway.Data.Interfaces;
using Blogsphere.Api.Gateway.Data.Interfaces.Base;
using Blogsphere.Api.Gateway.Entity;
using Blogsphere.Api.Gateway.Extensions;
using Blogsphere.Api.Gateway.Models.Common;
using Blogsphere.Api.Gateway.Models.Enums;
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

    public virtual async Task<Result<T>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.Here().MethodEntered();
        _logger.Debug("Getting {EntityType} by ID {Id}", typeof(T).Name, id);
        
        var entity = await _repository.GetByIdAsync(id, GetDefaultIncludes(), cancellationToken);
        
        if (entity == null || !entity.IsActive)
        {
            _logger.Warning("Active {EntityType} with ID {Id} not found", typeof(T).Name, id);
            return Result<T>.Failure(ErrorCodes.NotFound, $"Active {typeof(T).Name} with ID {id} not found");
        }
        
        _logger.Here().MethodExited(entity);
        return Result<T>.Success(entity);
    }

    public virtual async Task<Result<PaginatedResult<T>>> GetAllAsync(PaginationRequest request, CancellationToken cancellationToken = default)
    {
        _logger.Here().MethodEntered();
        _logger.Debug("Getting all {EntityType} with pagination {Request}", typeof(T).Name, request);
        
        var query = _repository.Include(GetDefaultIncludes());

        // Apply active status filter if specified
        if (request.IsActive.HasValue)
        {
            query = query.Where(e => e.IsActive == request.IsActive.Value);
        }

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
        else
        {
            // Default sort by UpdatedAt descending if no sort specified
            query = query.OrderByDescending(e => e.UpdatedAt);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        
        // Validate and adjust pagination parameters
        request.PageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        request.PageSize = request.PageSize switch
        {
            < 1 => 10,
            > 50 => 50,
            _ => request.PageSize
        };
        
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var result = new PaginatedResult<T>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
        
        _logger.Here().MethodExited(new { TotalCount = totalCount, PageCount = result.TotalPages });
        return Result<PaginatedResult<T>>.Success(result);
    }

    public virtual async Task<Result<T>> CreateAsync(T entity, RequestInformation requestInfo, CancellationToken cancellationToken = default)
    {
        _logger.Here().MethodEntered();
        _logger.Debug("Creating new {EntityType}", typeof(T).Name);
        
        entity.Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id;
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsActive = true;
        
        // Set audit fields from RequestInformation
        if (requestInfo?.CurrentUser?.Id != null)
        {
            entity.CreatedBy = requestInfo.CurrentUser.Id;
            entity.UpdatedBy = requestInfo.CurrentUser.Id;
        }
        
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.Information("Created {EntityType} with ID {Id}", typeof(T).Name, entity.Id);
        _logger.Here().MethodExited(entity);
        return Result<T>.Success(entity);
    }

    public virtual async Task<Result<T>> UpdateAsync(T entity, RequestInformation requestInfo, CancellationToken cancellationToken = default)
    {
        _logger.Here().MethodEntered();
        _logger.Debug("Updating {EntityType} with ID {Id}", typeof(T).Name, entity.Id);
        
        var existingEntity = await _repository.GetByIdAsync(entity.Id, cancellationToken);
        
        if (existingEntity == null)
        {
            _logger.Error("{EntityType} with ID {Id} not found for update", typeof(T).Name, entity.Id);
            return Result<T>.Failure(ErrorCodes.NotFound, $"{typeof(T).Name} with ID {entity.Id} not found");
        }

        // Preserve metadata
        entity.CreatedAt = existingEntity.CreatedAt;
        entity.CreatedBy = existingEntity.CreatedBy; // Preserve original creator
        entity.IsActive = existingEntity.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
        
        // Set UpdatedBy from RequestInformation
        if (requestInfo?.CurrentUser?.Id != null)
        {
            entity.UpdatedBy = requestInfo.CurrentUser.Id;
        }
        
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.Information("Updated {EntityType} with ID {Id}", typeof(T).Name, entity.Id);
        _logger.Here().MethodExited(entity);
        return Result<T>.Success(entity);
    }

    public virtual async Task<Result<bool>> DeleteAsync(Guid id, RequestInformation requestInfo, CancellationToken cancellationToken = default)
    {
        _logger.Here().MethodEntered();
        _logger.Debug("Deleting {EntityType} with ID {Id}", typeof(T).Name, id);
        
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        
        if (entity == null)
        {
            _logger.Error("{EntityType} with ID {Id} not found for deletion", typeof(T).Name, id);
            return Result<bool>.Failure(ErrorCodes.NotFound, $"{typeof(T).Name} with ID {id} not found");
        }

        // Perform soft delete
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        
        // Set UpdatedBy from RequestInformation for delete operation
        if (requestInfo?.CurrentUser?.Id != null)
        {
            entity.UpdatedBy = requestInfo.CurrentUser.Id;
        }
        
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.Information("Soft deleted {EntityType} with ID {Id}", typeof(T).Name, id);
        _logger.Here().MethodExited();
        return Result<bool>.Success(true);
    }

    // Backward compatibility methods for cases where RequestInformation is not available
    protected virtual async Task<Result<T>> CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        return await CreateAsync(entity, null, cancellationToken);
    }

    protected virtual async Task<Result<T>> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        return await UpdateAsync(entity, null, cancellationToken);
    }

    protected virtual async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DeleteAsync(id, null, cancellationToken);
    }

    protected virtual IQueryable<T> ApplySearch(IQueryable<T> query, string searchTerm)
    {
        return query;
    }

    protected virtual IQueryable<T> ApplySort(IQueryable<T> query, string sortBy, bool isDescending)
    {
        return query;
    }

    protected virtual Expression<Func<T, object>>[] GetDefaultIncludes()
    {
        return [];
    }
} 
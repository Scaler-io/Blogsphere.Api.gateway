using System.Linq.Expressions;
using AutoMapper;
using Blogsphere.Api.Gateway.Data.Interfaces;
using Blogsphere.Api.Gateway.Data.Interfaces.Repositories;
using Blogsphere.Api.Gateway.Entity;
using Blogsphere.Api.Gateway.Extensions;
using Blogsphere.Api.Gateway.Models.Common;
using Blogsphere.Api.Gateway.Models.DTOs;
using Blogsphere.Api.Gateway.Models.DTOs.Search;
using Blogsphere.Api.Gateway.Models.Enums;
using Blogsphere.Api.Gateway.Models.Requests;
using Blogsphere.Api.Gateway.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blogsphere.Api.Gateway.Services;

public class ProxyClusterService(
    ILogger logger,
    IProxyClusterRepository repository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IProxyClusterService
{
    private readonly IProxyClusterRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger _logger = logger.ForContext<ProxyClusterService>();

    public async Task<Result<bool>> AnyAsync()
    {
        _logger.Here().MethodEntered();
        var result = await _repository.AsQueryable().AnyAsync();
        _logger.Here().MethodExited();
        return Result<bool>.Success(result);
    }

    public async Task<Result<ProxyClusterDto>> GetByClusterIdAsync(string clusterId)
    {
        _logger.Here().MethodEntered();
        var entity = await IncludeAllRelationships(_repository.AsQueryable())
            .FirstOrDefaultAsync(c => c.ClusterId == clusterId);
        
        if (entity == null)
        {
            _logger.Here().Warning("Cluster with ClusterId {ClusterId} not found", clusterId);
            return Result<ProxyClusterDto>.Failure(ErrorCodes.NotFound, $"Cluster with ClusterId {clusterId} not found");
        }

        var dto = _mapper.Map<ProxyClusterDto>(entity);
        _logger.Here().MethodExited();
        return Result<ProxyClusterDto>.Success(dto);
    }

    public async Task<Result<PaginatedResult<ProxyClusterSearchableDto>>> GetAllAsync(PaginationRequest request)
    {
        _logger.Here().MethodEntered();
        var query = IncludeAllRelationships(_repository.AsQueryable());
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var dtos = _mapper.Map<List<ProxyClusterSearchableDto>>(items);
        var result = new PaginatedResult<ProxyClusterSearchableDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        _logger.Here().Information("Found {TotalCount} clusters", totalCount);
        _logger.Here().MethodExited();
        return Result<PaginatedResult<ProxyClusterSearchableDto>>.Success(result);
    }

    public async Task<Result<ProxyClusterDto>> GetByIdAsync(Guid id)
    {
        _logger.Here().MethodEntered();
        var entity = await IncludeAllRelationships(_repository.AsQueryable())
            .FirstOrDefaultAsync(c => c.Id == id);
            
        if (entity == null)
        {
            _logger.Here().Warning("Cluster with ID {Id} not found", id);
            return Result<ProxyClusterDto>.Failure(ErrorCodes.NotFound, $"Cluster with ID {id} not found");
        }

        var dto = _mapper.Map<ProxyClusterDto>(entity);
        _logger.Here().MethodExited();
        return Result<ProxyClusterDto>.Success(dto);
    }

    public async Task<Result<ProxyClusterDto>> CreateAsync(ProxyClusterDto dto, RequestInformation requestInfo)
    {
        _logger.Here().MethodEntered();
        var entity = _mapper.Map<ProxyCluster>(dto);
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        
        // Set audit fields from RequestInformation
        if (requestInfo?.CurrentUser?.Id != null)
        {
            entity.CreatedBy = requestInfo.CurrentUser.Id;
            entity.UpdatedBy = requestInfo.CurrentUser.Id;
        }
        
        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        
        var resultDto = _mapper.Map<ProxyClusterDto>(entity);
        _logger.Here().Information("Created cluster with ID {Id}", entity.Id);
        _logger.Here().MethodExited();
        return Result<ProxyClusterDto>.Success(resultDto);
    }

    public async Task<Result<ProxyClusterDto>> UpdateAsync(Guid id, ProxyClusterDto dto, RequestInformation requestInfo)
    {
        _logger.Here().MethodEntered();
        
        // Check if entity exists first
        var exists = await _repository.AsQueryable().AnyAsync(c => c.Id == id);
        if (!exists)
        {
            _logger.Here().Warning("Cluster with ID {Id} not found for update", id);
            return Result<ProxyClusterDto>.Failure(ErrorCodes.NotFound, $"Cluster with ID {id} not found");
        }

        // Update main cluster properties using direct approach
        await UpdateClusterPropertiesAsync(id, dto, requestInfo);

        // Handle destinations update separately if provided
        if (dto.Destinations != null)
        {
            await UpdateClusterDestinationsAsync(id, dto.Destinations, requestInfo);
        }
        
        // Get updated entity to return
        var updatedEntity = await IncludeAllRelationships(_repository.AsQueryable())
            .FirstOrDefaultAsync(c => c.Id == id);
            
        var resultDto = _mapper.Map<ProxyClusterDto>(updatedEntity);
        _logger.Here().Information("Updated cluster with ID {Id}", id);
        _logger.Here().MethodExited();
        return Result<ProxyClusterDto>.Success(resultDto);
    }

    public async Task<Result<ProxyClusterDto>> CreateFromRequestAsync(CreateProxyClusterRequest request, RequestInformation requestInfo)
    {
        _logger.Here().MethodEntered();
        
        // Map request to DTO using AutoMapper
        var dto = _mapper.Map<ProxyClusterDto>(request);
        
        // Set system properties
        dto.Id = Guid.NewGuid();
        dto.IsActive = true;
        dto.CreatedAt = DateTime.UtcNow;
        dto.UpdatedAt = DateTime.UtcNow;

        // Map destinations if provided
        if (request.Destinations != null)
        {
            dto.Destinations = _mapper.Map<List<ProxyDestinationDto>>(request.Destinations);
            foreach (var dest in dto.Destinations)
            {
                dest.Id = Guid.NewGuid();
                dest.ClusterId = dto.Id;
                dest.IsActive = true;
                dest.CreatedAt = DateTime.UtcNow;
                dest.UpdatedAt = DateTime.UtcNow;
                dest.CreatedBy = requestInfo.CurrentUser.Id;
                dest.UpdatedBy = requestInfo.CurrentUser.Id;
            }
        }

        // Use the existing CreateAsync method
        var result = await CreateAsync(dto, requestInfo);
        _logger.Here().MethodExited();
        return result;
    }

    public async Task<Result<ProxyClusterDto>> UpdateFromRequestAsync(Guid id, UpdateProxyClusterRequest request, RequestInformation requestInfo)
    {
        _logger.Here().MethodEntered();
        
        // Check if entity exists without loading all related data
        var exists = await _repository.AsQueryable().AnyAsync(c => c.Id == id);
        if (!exists)
        {
            _logger.Here().Warning("Cluster with ID {Id} not found for update", id);
            return Result<ProxyClusterDto>.Failure(ErrorCodes.NotFound, $"Cluster with ID {id} not found");
        }

        // Map request to DTO using AutoMapper
        var dto = _mapper.Map<ProxyClusterDto>(request);
        dto.Id = id;
        dto.UpdatedAt = DateTime.UtcNow;

        // Handle explicit destination removals first
        if (request.RemoveDestinations != null && request.RemoveDestinations.Any())
        {
            await RemoveClusterDestinationsAsync(id, request.RemoveDestinations, requestInfo);
        }

        // Map destinations if provided
        if (request.Destinations != null)
        {
            dto.Destinations = _mapper.Map<List<ProxyDestinationDto>>(request.Destinations);
            foreach (var dest in dto.Destinations)
            {
                dest.ClusterId = id;
            }
        }

        // Use the smart update logic
        return await UpdateAsync(id, dto, requestInfo);
    }

    private async Task UpdateClusterPropertiesAsync(Guid id, ProxyClusterDto dto, RequestInformation requestInfo)
    {
        // Load only the cluster entity for updating basic properties
        var entity = await _repository.AsQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
            
        if (entity == null) return;

        // Update only provided cluster properties (null values are ignored)
        if (dto.ClusterId != null) entity.ClusterId = dto.ClusterId;
        if (dto.LoadBalancingPolicy != null) entity.LoadBalancingPolicy = dto.LoadBalancingPolicy;
        entity.HealthCheckEnabled = dto.HealthCheckEnabled;
        if (dto.HealthCheckPath != null) entity.HealthCheckPath = dto.HealthCheckPath;
        entity.HealthCheckInterval = dto.HealthCheckInterval;
        entity.HealthCheckTimeout = dto.HealthCheckTimeout;
        entity.UpdatedAt = DateTime.UtcNow;
        
        // Set UpdatedBy from RequestInformation
        if (requestInfo?.CurrentUser?.Id != null)
        {
            entity.UpdatedBy = requestInfo.CurrentUser.Id;
        }

        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task UpdateClusterDestinationsAsync(Guid clusterId, ICollection<ProxyDestinationDto> newDestinations, RequestInformation requestInfo)
    {
        // Get existing destinations for this cluster
        var existingDestinations = await _unitOfWork.Destinations.AsQueryable()
            .Where(d => d.ClusterId == clusterId)
            .ToListAsync();

        foreach (var destinationDto in newDestinations)
        {
            var existingDestination = existingDestinations.FirstOrDefault(d => d.DestinationId == destinationDto.DestinationId);

            if (existingDestination != null)
            {
                // Update existing destination
                if (destinationDto.Address != null) existingDestination.Address = destinationDto.Address;
                existingDestination.IsActive = destinationDto.IsActive;
                existingDestination.UpdatedAt = DateTime.UtcNow;
                
                // Set UpdatedBy from RequestInformation
                if (requestInfo?.CurrentUser?.Id != null)
                {
                    existingDestination.UpdatedBy = requestInfo.CurrentUser.Id;
                }
                
                _unitOfWork.Destinations.Update(existingDestination);
            }
            else
            {
                // Add new destination
                var newDestination = new ProxyDestination
                {
                    Id = Guid.NewGuid(),
                    DestinationId = destinationDto.DestinationId,
                    Address = destinationDto.Address,
                    ClusterId = clusterId,
                    IsActive = destinationDto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                
                // Set audit fields from RequestInformation
                if (requestInfo?.CurrentUser?.Id != null)
                {
                    newDestination.CreatedBy = requestInfo.CurrentUser.Id;
                    newDestination.UpdatedBy = requestInfo.CurrentUser.Id;
                }
                
                await _unitOfWork.Destinations.AddAsync(newDestination);
            }
        }

        // Permanently delete destinations that are not in the new list
        var providedDestinationIds = newDestinations.Select(d => d.DestinationId).ToHashSet();
        var destinationsToDelete = existingDestinations.Where(d => !providedDestinationIds.Contains(d.DestinationId)).ToList();
        
        foreach (var destinationToDelete in destinationsToDelete)
        {
            _unitOfWork.Destinations.Remove(destinationToDelete);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task RemoveClusterDestinationsAsync(Guid clusterId, ICollection<string> destinationIdsToRemove, RequestInformation requestInfo)
    {
        var destinationsToRemove = await _unitOfWork.Destinations.AsQueryable()
            .Where(d => d.ClusterId == clusterId && destinationIdsToRemove.Contains(d.DestinationId))
            .ToListAsync();

        foreach (var destination in destinationsToRemove)
        {
            _unitOfWork.Destinations.Remove(destination);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, RequestInformation requestInfo)
    {
        _logger.Here().MethodEntered();
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            _logger.Here().Warning("Cluster with ID {Id} not found for deletion", id);
            return Result<bool>.Failure(ErrorCodes.NotFound, $"Cluster with ID {id} not found");
        }

        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        
        // Set UpdatedBy from RequestInformation for delete operation
        if (requestInfo?.CurrentUser?.Id != null)
        {
            entity.UpdatedBy = requestInfo.CurrentUser.Id;
        }
        
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        
        _logger.Here().Information("Soft deleted cluster with ID {Id}", id);
        _logger.Here().MethodExited();
        return Result<bool>.Success(true);
    }

    private Expression<Func<ProxyCluster, object>>[] GetDefaultIncludes()
    {
        return
        [
            c => c.Destinations,
            c => c.Routes.Where(r => r.IsActive)
        ];
    }
    
    // Override to include nested relationships for routes
    private IQueryable<ProxyCluster> IncludeAllRelationships(IQueryable<ProxyCluster> query)
    {
        return query
            .Include(c => c.Destinations)
            .Include(c => c.Routes.Where(r => r.IsActive))
                .ThenInclude(r => r.Headers)
            .Include(c => c.Routes.Where(r => r.IsActive))
                .ThenInclude(r => r.Transforms);
    }
} 
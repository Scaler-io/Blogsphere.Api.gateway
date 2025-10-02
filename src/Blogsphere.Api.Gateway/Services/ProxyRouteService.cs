using System.Linq.Expressions;
using AutoMapper;
using Contracts.Events;
using Microsoft.EntityFrameworkCore;

namespace Blogsphere.Api.Gateway.Services;

public class ProxyRouteService(
    ILogger logger,
    IProxyRouteRepository repository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IPublishServiceFactory publishServiceFactory) : IProxyRouteService
{
    private readonly IProxyRouteRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger _logger = logger.ForContext<ProxyRouteService>();
    private readonly IPublishServiceFactory _publishServiceFactory = publishServiceFactory;

    public async Task<Result<bool>> AnyAsync()
    {
        _logger.Here().MethodEntered();
        var result = await _repository.AsQueryable().AnyAsync();
        _logger.Here().MethodExited();
        return Result<bool>.Success(result);
    }

    public async Task<Result<PaginatedResult<ProxyRouteSearchableDto>>> GetAllAsync(PaginationRequest request)
    {
        _logger.Here().MethodEntered();
        var query = _repository.Include(GetDefaultIncludes());
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var dtos = _mapper.Map<List<ProxyRouteSearchableDto>>(items);
        var result = new PaginatedResult<ProxyRouteSearchableDto>   
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
        
        _logger.Here().Information("Found {TotalCount} routes", totalCount);
        _logger.Here().MethodExited();
        return Result<PaginatedResult<ProxyRouteSearchableDto>>.Success(result);
    }


    public async Task<Result<ProxyRouteDto>> GetByIdAsync(Guid id)
    {
        _logger.Here().MethodEntered();
        var entity = await _repository.Include(GetDefaultIncludes()).FirstOrDefaultAsync(r => r.Id == id);
        if (entity == null)
        {
            _logger.Here().Warning("Route with ID {Id} not found", id);
            return Result<ProxyRouteDto>.Failure(ErrorCodes.NotFound, $"Route with ID {id} not found");
        }

        var dto = _mapper.Map<ProxyRouteDto>(entity);
        _logger.Here().MethodExited();
        return Result<ProxyRouteDto>.Success(dto);
    }

    public async Task<Result<ProxyRouteDto>> CreateAsync(ProxyRouteDto dto, RequestInformation requestInfo)
    {
        _logger.Here().MethodEntered();
        
        // Validate ClusterId exists if provided
        if (dto.ClusterId != Guid.Empty)
        {
            var clusterExists = await _unitOfWork.Clusters.AsQueryable()
                .AnyAsync(c => c.Id == dto.ClusterId && c.IsActive);
            
            if (!clusterExists)
            {
                _logger.Here().Warning("Cluster with ID {ClusterId} not found for route creation", dto.ClusterId);
                return Result<ProxyRouteDto>.Failure(ErrorCodes.BadRequest, $"Cluster with ID {dto.ClusterId} not found");
            }
        }
        
        var entity = _mapper.Map<ProxyRoute>(dto);
        
        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        
        // Map to DTO for response
        var resultDto = _mapper.Map<ProxyRouteDto>(entity);
        
        // Publish route created event
        await _publishServiceFactory.CreatePublishServiceAsync<ProxyRoute, ApiRouteCreated>()
            .PublishAsync(entity, requestInfo.CorrelationId);

        // Get cluster for update event (optimized query - only load what's needed for the event)
        var cluster = await _unitOfWork.Clusters.AsQueryable()
            .Include(c => c.Routes)
            .Include(c => c.Destinations)
            .FirstOrDefaultAsync(c => c.Id == entity.ClusterId);
            
        if (cluster != null)
        {
            await _publishServiceFactory.CreatePublishServiceAsync<ProxyCluster, ApiClusterUpdated>()
                .PublishAsync(cluster, requestInfo.CorrelationId);
        }

        _logger.Here().Information("Created route with ID {Id}", entity.Id);
        _logger.Here().MethodExited();      
        return Result<ProxyRouteDto>.Success(resultDto);
    }

    public async Task<Result<ProxyRouteDto>> UpdateAsync(Guid id, ProxyRouteDto dto, RequestInformation requestInfo)
    {
        _logger.Here().MethodEntered();
        
        // Check if entity exists first
        var exists = await _repository.AsQueryable().AnyAsync(r => r.Id == id);
        if (!exists)
        {
            _logger.Here().Warning("Route with ID {Id} not found for update", id);
            return Result<ProxyRouteDto>.Failure(ErrorCodes.NotFound, $"Route with ID {id} not found");
        }

        // Handle ClusterId validation first if it's being updated
        if (dto.ClusterId != Guid.Empty)
        {
            var clusterExists = await _unitOfWork.Clusters.AsQueryable()
                .AnyAsync(c => c.Id == dto.ClusterId && c.IsActive);
            
            if (!clusterExists)
            {
                _logger.Here().Warning("Cluster with ID {ClusterId} not found for route update", dto.ClusterId);
                return Result<ProxyRouteDto>.Failure(ErrorCodes.BadRequest, $"Cluster with ID {dto.ClusterId} not found");
            }
        }

        // Update main route properties using direct SQL approach
        await UpdateRoutePropertiesAsync(id, dto, requestInfo);

        // Handle headers update separately if provided
        if (dto.Headers != null)
        {
            await UpdateRouteHeadersAsync(id, dto.Headers, requestInfo);
        }

        // Handle transforms update separately if provided
        if (dto.Transforms != null)
        {
            await UpdateRouteTransformsAsync(id, dto.Transforms, requestInfo);
        }
        
        // Get updated entity to return
        var updatedEntity = await _repository.AsQueryable()
            .Include(r => r.Headers)
            .Include(r => r.Transforms)
            .Include(r => r.Cluster)
            .FirstOrDefaultAsync(r => r.Id == id);

        await _publishServiceFactory.CreatePublishServiceAsync<ProxyRoute, ApiRouteUpdated>()
        .PublishAsync(updatedEntity, requestInfo.CorrelationId);

        var resultDto = _mapper.Map<ProxyRouteDto>(updatedEntity);
        _logger.Here().Information("Updated route with ID {Id}", id);
        _logger.Here().MethodExited();
        return Result<ProxyRouteDto>.Success(resultDto);
    }

    public async Task<Result<ProxyRouteDto>> CreateFromRequestAsync(CreateProxyRouteRequest request, RequestInformation requestInfo)
    {
        _logger.Here().MethodEntered();
        
        // Map request to DTO using AutoMapper
        var dto = _mapper.Map<ProxyRouteDto>(request);
        
        // Set system properties
        dto.Id = Guid.NewGuid();
        dto.IsActive = request.IsActive;
        dto.Metadata = new MetaDataDto
        {
            CreatedBy = requestInfo.CurrentUser.Id,
            UpdatedBy = requestInfo.CurrentUser.Id
        };

        // Map headers if provided
        if (request.Headers != null)
        {
            dto.Headers = _mapper.Map<List<ProxyHeaderDto>>(request.Headers);
            foreach (var header in dto.Headers)
            {
                header.Id = Guid.NewGuid();
                header.IsActive = request.IsActive;
                header.Metadata = new MetaDataDto
                {
                    CreatedBy = requestInfo.CurrentUser.Id,
                    UpdatedBy = requestInfo.CurrentUser.Id
                };
            }
        }

        // Map transforms if provided
        if (request.Transforms != null)
        {
            dto.Transforms = _mapper.Map<List<ProxyTransformDto>>(request.Transforms);
            foreach (var transform in dto.Transforms)
            {
                transform.Id = Guid.NewGuid();
                transform.IsActive = request.IsActive;
                transform.Metadata = new MetaDataDto
                {
                    CreatedBy = requestInfo.CurrentUser.Id,
                    UpdatedBy = requestInfo.CurrentUser.Id
                };
            }
        }

        // Use the existing CreateAsync method
        var result = await CreateAsync(dto, requestInfo);
        _logger.Here().MethodExited();
        return result;
    }

    public async Task<Result<ProxyRouteDto>> UpdateFromRequestAsync(Guid id, UpdateProxyRouteRequest request, RequestInformation requestInfo)
    {
        _logger.Here().MethodEntered();
        
        // Check if entity exists without loading all related data
        var exists = await _repository.AsQueryable().AnyAsync(r => r.Id == id);
        if (!exists)
        {
            _logger.Here().Warning("Route with ID {Id} not found for update", id);
            return Result<ProxyRouteDto>.Failure(ErrorCodes.NotFound, $"Route with ID {id} not found");
        }

        // Map request to DTO using AutoMapper
        var dto = _mapper.Map<ProxyRouteDto>(request);
        dto.Id = id;
        dto.IsActive = request.IsActive;
        dto.Metadata = new MetaDataDto
        {
            UpdatedBy = requestInfo.CurrentUser.Id
        };
        
        // Handle nullable ClusterId properly
        if (request.ClusterId.HasValue)
        {
            dto.ClusterId = request.ClusterId.Value;
        }

        // Handle explicit removals first
        if (request.RemoveHeaders != null && request.RemoveHeaders.Any())
        {
            await RemoveRouteHeadersAsync(id, request.RemoveHeaders, requestInfo);
        }

        if (request.RemoveTransforms != null && request.RemoveTransforms.Any())
        {
            await RemoveRouteTransformsAsync(id, request.RemoveTransforms, requestInfo);
        }

        // Map headers if provided
        if (request.Headers != null)
        {
            dto.Headers = _mapper.Map<List<ProxyHeaderDto>>(request.Headers);
            foreach (var header in dto.Headers)
            {
                header.Metadata = new MetaDataDto
                {
                    UpdatedBy = requestInfo.CurrentUser.Id
                };
            }
        }

        // Map transforms if provided
        if (request.Transforms != null)
        {
            dto.Transforms = _mapper.Map<List<ProxyTransformDto>>(request.Transforms);
            foreach (var transform in dto.Transforms)
            {
                transform.Metadata = new MetaDataDto
                {
                    UpdatedBy = requestInfo.CurrentUser.Id
                };
            }
        }

        // Use the smart update logic
        return await UpdateAsync(id, dto, requestInfo);
    }

    private async Task UpdateRoutePropertiesAsync(Guid id, ProxyRouteDto dto, RequestInformation requestInfo)
    {
        // Load only the route entity for updating basic properties
        var entity = await _repository.AsQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);
            
        if (entity == null) return;

        // Update only provided route properties (null values are ignored)
        if (dto.RouteId != null) entity.RouteId = dto.RouteId;
        if (dto.Path != null) entity.Path = dto.Path;
        if (dto.Methods != null) entity.Methods = dto.Methods;
        if (dto.RateLimiterPolicy != null) entity.RateLimiterPolicy = dto.RateLimiterPolicy;
        if (dto.ClusterId != Guid.Empty) entity.ClusterId = dto.ClusterId;
        entity.IsActive = dto.IsActive;

        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task UpdateRouteHeadersAsync(Guid routeId, ICollection<ProxyHeaderDto> newHeaders, RequestInformation requestInfo)
    {
        // Get existing headers for this route
        var existingHeaders = await _unitOfWork.Headers.AsQueryable()
            .Where(h => h.RouteId == routeId)
            .ToListAsync();

        foreach (var headerDto in newHeaders)
        {
            var existingHeader = existingHeaders.FirstOrDefault(h => h.Name == headerDto.Name);

            if (existingHeader != null)
            {
                // Update existing header
                existingHeader.Values = headerDto.Values ?? existingHeader.Values;
                existingHeader.Mode = headerDto.Mode ?? existingHeader.Mode;
                existingHeader.IsActive = headerDto.IsActive;

                
                _unitOfWork.Headers.Update(existingHeader);
            }
            else
            {
                // Add new header
                var newHeader = new ProxyHeader
                {
                    Id = Guid.NewGuid(),
                    Name = headerDto.Name,
                    Values = headerDto.Values,
                    Mode = headerDto.Mode,
                    RouteId = routeId,
                    IsActive = headerDto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = requestInfo.CurrentUser.Id,
                    UpdatedBy = requestInfo.CurrentUser.Id
                };
                
                await _unitOfWork.Headers.AddAsync(newHeader);
            }
        }

        // Permanently delete headers that are not in the new list
        var providedHeaderNames = newHeaders.Select(h => h.Name).ToHashSet();
        var headersToDelete = existingHeaders.Where(h => !providedHeaderNames.Contains(h.Name)).ToList();
        
        foreach (var headerToDelete in headersToDelete)
        {
            _unitOfWork.Headers.Remove(headerToDelete);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task UpdateRouteTransformsAsync(Guid routeId, ICollection<ProxyTransformDto> newTransforms, RequestInformation requestInfo)
    {
        // Get existing transforms for this route
        var existingTransforms = await _unitOfWork.Transforms.AsQueryable()
            .Where(t => t.RouteId == routeId)
            .ToListAsync();

        foreach (var transformDto in newTransforms)
        {
            var existingTransform = existingTransforms.FirstOrDefault(t => t.PathPattern == transformDto.PathPattern);

            if (existingTransform != null)
            {
                // Update existing transform
                existingTransform.IsActive = transformDto.IsActive;
                
                _unitOfWork.Transforms.Update(existingTransform);
            }
            else
            {
                // Add new transform
                var newTransform = new ProxyTransform
                {
                    Id = Guid.NewGuid(),
                    PathPattern = transformDto.PathPattern,
                    RouteId = routeId,
                    IsActive = transformDto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = requestInfo.CurrentUser.Id,
                    UpdatedBy = requestInfo.CurrentUser.Id
                };     
                
                await _unitOfWork.Transforms.AddAsync(newTransform);
            }
        }

        // Permanently delete transforms that are not in the new list
        var providedPathPatterns = newTransforms.Select(t => t.PathPattern).ToHashSet();
        var transformsToDelete = existingTransforms.Where(t => !providedPathPatterns.Contains(t.PathPattern)).ToList();
        
        foreach (var transformToDelete in transformsToDelete)
        {
            _unitOfWork.Transforms.Remove(transformToDelete);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task RemoveRouteHeadersAsync(Guid routeId, ICollection<string> headerNamesToRemove, RequestInformation requestInfo)
    {
        var headersToRemove = await _unitOfWork.Headers.AsQueryable()
            .Where(h => h.RouteId == routeId && headerNamesToRemove.Contains(h.Name))
            .ToListAsync();

        foreach (var header in headersToRemove)
        {
            _unitOfWork.Headers.Remove(header);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task RemoveRouteTransformsAsync(Guid routeId, ICollection<string> transformPatternsToRemove, RequestInformation requestInfo)
    {
        var transformsToRemove = await _unitOfWork.Transforms.AsQueryable()
            .Where(t => t.RouteId == routeId && transformPatternsToRemove.Contains(t.PathPattern))
            .ToListAsync();

        foreach (var transform in transformsToRemove)
        {
            _unitOfWork.Transforms.Remove(transform);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, RequestInformation requestInfo)
    {
        _logger.Here().MethodEntered();
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            _logger.Here().Warning("Route with ID {Id} not found for deletion", id);
            return Result<bool>.Failure(ErrorCodes.NotFound, $"Route with ID {id} not found");
        }

        var clusterId = entity.ClusterId;        
        _repository.Remove(entity);
        await _unitOfWork.SaveChangesAsync();
        
        var updatedCluster = await _unitOfWork.Clusters.AsQueryable()
            .Include(c => c.Routes)
            .Include(c => c.Destinations)
            .FirstOrDefaultAsync(c => c.Id == clusterId);

        await _publishServiceFactory.CreatePublishServiceAsync<ProxyRoute, ApiRouteDeleted>()
        .PublishAsync(entity, requestInfo.CorrelationId);
    

        await _publishServiceFactory.CreatePublishServiceAsync<ProxyCluster, ApiClusterUpdated>()
        .PublishAsync(updatedCluster, requestInfo.CorrelationId);

        
        _logger.Here().Information("Deleted route with ID {Id}", id);
        _logger.Here().MethodExited();
        return Result<bool>.Success(true);
    }

    private static Expression<Func<ProxyRoute, object>>[] GetDefaultIncludes()
    {
        return
        [
            r => r.Headers,
            r => r.Transforms,
            r => r.Cluster
        ];
    }
} 
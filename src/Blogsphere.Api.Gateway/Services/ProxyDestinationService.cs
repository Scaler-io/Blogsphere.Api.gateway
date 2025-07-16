using System.Linq.Expressions;
using AutoMapper;
using Blogsphere.Api.Gateway.Data.Interfaces;
using Blogsphere.Api.Gateway.Data.Interfaces.Repositories;
using Blogsphere.Api.Gateway.Entity;
using Blogsphere.Api.Gateway.Extensions;
using Blogsphere.Api.Gateway.Models.Common;
using Blogsphere.Api.Gateway.Models.DTOs;
using Blogsphere.Api.Gateway.Models.Enums;
using Blogsphere.Api.Gateway.Services.Base;
using Blogsphere.Api.Gateway.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blogsphere.Api.Gateway.Services;

public class ProxyDestinationService(
    ILogger logger,
    IProxyDestinationRepository repository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : BaseService<ProxyDestination>(logger, repository, unitOfWork), IProxyDestinationService
{
    private new readonly IProxyDestinationRepository _repository = repository;
    private new readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private new readonly ILogger _logger = logger.ForContext<ProxyDestinationService>();

    public async Task<Result<PaginatedResult<ProxyDestinationDto>>> GetAllAsync(PaginationRequest request)
    {
        _logger.Here().MethodEntered();
        var query = _repository.Include(GetDefaultIncludes());
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var dtos = _mapper.Map<List<ProxyDestinationDto>>(items);
        var result = new PaginatedResult<ProxyDestinationDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        _logger.Here().MethodExited();
        return Result<PaginatedResult<ProxyDestinationDto>>.Success(result);
    }

    public async Task<Result<ProxyDestinationDto>> GetByIdAsync(Guid id)
    {
        _logger.Here().MethodEntered();
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            _logger.Here().Warning("Destination with ID {Id} not found", id);
            return Result<ProxyDestinationDto>.Failure(ErrorCodes.NotFound, $"Destination with ID {id} not found");
        }

        var dto = _mapper.Map<ProxyDestinationDto>(entity);
        _logger.Here().MethodExited();
        return Result<ProxyDestinationDto>.Success(dto);
    }

    public async Task<Result<ProxyDestinationDto>> CreateAsync(ProxyDestinationDto dto)
    {
        _logger.Here().MethodEntered();
        var entity = _mapper.Map<ProxyDestination>(dto);
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        
        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        
        var resultDto = _mapper.Map<ProxyDestinationDto>(entity);
        _logger.Here().Information("Created destination with ID {Id}", entity.Id);
        _logger.Here().MethodExited();
        return Result<ProxyDestinationDto>.Success(resultDto);
    }

    public async Task<Result<ProxyDestinationDto>> UpdateAsync(Guid id, ProxyDestinationDto dto)
    {
        _logger.Here().MethodEntered();
        
        // Load entity without tracking to avoid concurrency issues
        var entity = await _repository.AsQueryable()
            .AsNoTracking()
            .Include(d => d.Cluster)
            .FirstOrDefaultAsync(d => d.Id == id);
            
        if (entity == null)
        {
            _logger.Here().Warning("Destination with ID {Id} not found for update", id);
            return Result<ProxyDestinationDto>.Failure(ErrorCodes.NotFound, $"Destination with ID {id} not found");
        }

        _mapper.Map(dto, entity);
        entity.UpdatedAt = DateTime.UtcNow;
        
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        
        var resultDto = _mapper.Map<ProxyDestinationDto>(entity);
        _logger.Here().Information("Updated destination with ID {Id}", entity.Id);
        _logger.Here().MethodExited();
        return Result<ProxyDestinationDto>.Success(resultDto);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        _logger.Here().MethodEntered();
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            _logger.Here().Warning("Destination with ID {Id} not found for deletion", id);
            return Result<bool>.Failure(ErrorCodes.NotFound, $"Destination with ID {id} not found");
        }

        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        
        _logger.Here().Information("Soft deleted destination with ID {Id}", id);
        _logger.Here().MethodExited();
        return Result<bool>.Success(true);
    }

    protected override Expression<Func<ProxyDestination, object>>[] GetDefaultIncludes()
    {
        return
        [
            d => d.Cluster
        ];
    }
} 
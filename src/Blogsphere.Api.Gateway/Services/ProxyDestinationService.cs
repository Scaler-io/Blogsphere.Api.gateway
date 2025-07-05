using System.Linq.Expressions;
using Blogsphere.Api.Gateway.Data.Interfaces;
using Blogsphere.Api.Gateway.Data.Interfaces.Repositories;
using Blogsphere.Api.Gateway.Entity;
using Blogsphere.Api.Gateway.Extensions;
using Blogsphere.Api.Gateway.Services.Base;
using Blogsphere.Api.Gateway.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blogsphere.Api.Gateway.Services;

public class ProxyDestinationService(
    ILogger logger,
    IProxyDestinationRepository repository,
    IUnitOfWork unitOfWork) : BaseService<ProxyDestination>(logger, repository, unitOfWork), IProxyDestinationService
{
    public async Task<ProxyDestination> GetByDestinationIdAsync(string destinationId, CancellationToken cancellationToken = default)
    {
        _logger.Here().MethodEntered();
        
        try
        {
            var destination = await _repository.Include(GetDefaultIncludes())
                .FirstOrDefaultAsync(d => d.DestinationId == destinationId, cancellationToken);

            if (destination == null)
            {
                _logger.Here().Warning("Destination with ID {DestinationId} not found", destinationId);
                throw new InvalidOperationException($"Destination with ID {destinationId} not found");
            }

            _logger.Here().MethodExited(destination);
            return destination;
        }
        catch (Exception ex)
        {
            _logger.Here().Error(ex, "Error getting destination by ID {DestinationId}", destinationId);
            throw;
        }
    }

    public async Task<ProxyDestination> CreateAsync(ProxyDestination destination, CancellationToken cancellationToken = default)
    {
        _logger.Here().MethodEntered();
        
        try
        {
            await _repository.AddAsync(destination, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.Here().Information("Created destination with ID {DestinationId}", destination.DestinationId);
            _logger.Here().MethodExited(destination);
            return destination;
        }
        catch (Exception ex)
        {
            _logger.Here().Error(ex, "Error creating destination with ID {DestinationId}", destination.DestinationId);
            throw;
        }
    }

    public async Task<IEnumerable<ProxyDestination>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.Here().MethodEntered();
        
        try
        {
            var destinations = await _repository.Include(GetDefaultIncludes())
                .ToListAsync(cancellationToken);

            _logger.Here().Information("Retrieved {Count} destinations", destinations.Count);
            _logger.Here().MethodExited(destinations);
            return destinations;
        }
        catch (Exception ex)
        {
            _logger.Here().Error(ex, "Error getting all destinations");
            throw;
        }
    }

    public async Task<IEnumerable<ProxyDestination>> GetByClusterIdAsync(Guid clusterId, CancellationToken cancellationToken = default)
    {
        _logger.Here().MethodEntered();
        
        try
        {
            var destinations = await _repository.Include(GetDefaultIncludes())
                .Where(d => d.ClusterId == clusterId)
                .ToListAsync(cancellationToken);

            if (!destinations.Any())
            {
                _logger.Here().Warning("No destinations found for cluster ID {ClusterId}", clusterId);
            }

            _logger.Here().MethodExited(destinations);
            return destinations;
        }
        catch (Exception ex)
        {
            _logger.Here().Error(ex, "Error getting destinations by cluster ID {ClusterId}", clusterId);
            throw;
        }
    }

    protected override Expression<Func<ProxyDestination, object>>[] GetDefaultIncludes()
    {
        return
        [
            d => d.Cluster
        ];
    }
} 
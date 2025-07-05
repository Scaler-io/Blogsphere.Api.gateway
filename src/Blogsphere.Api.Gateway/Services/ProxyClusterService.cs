using System.Linq.Expressions;
using Blogsphere.Api.Gateway.Data.Interfaces;
using Blogsphere.Api.Gateway.Data.Interfaces.Repositories;
using Blogsphere.Api.Gateway.Entity;
using Blogsphere.Api.Gateway.Extensions;
using Blogsphere.Api.Gateway.Services.Base;
using Blogsphere.Api.Gateway.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blogsphere.Api.Gateway.Services;

public class ProxyClusterService(
    ILogger logger,
    IProxyClusterRepository repository,
    IUnitOfWork unitOfWork) : BaseService<ProxyCluster>(logger, repository, unitOfWork), IProxyClusterService
{
    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        _logger.Here().MethodEntered();
        
        try
        {
            var result = await _repository.AnyAsync(x => true, cancellationToken);
            _logger.Here().MethodExited(result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.Here().Error(ex, "Error checking if any clusters exist");
            throw;
        }
    }

    public async Task<IEnumerable<ProxyCluster>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.Here().MethodEntered();
        
        try
        {
            var clusters = await _repository.Include(GetDefaultIncludes())
                .ToListAsync(cancellationToken);

            _logger.Here().Information("Retrieved {Count} clusters", clusters.Count);
            _logger.Here().MethodExited(clusters);
            return clusters;
        }
        catch (Exception ex)
        {
            _logger.Here().Error(ex, "Error getting all clusters");
            throw;
        }
    }

    public async Task<ProxyCluster> CreateAsync(ProxyCluster cluster, CancellationToken cancellationToken = default)
    {
        _logger.Here().MethodEntered();
        
        try
        {
            await _repository.AddAsync(cluster, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.Here().Information("Created cluster with ID {ClusterId}", cluster.ClusterId);
            _logger.Here().MethodExited(cluster);
            return cluster;
        }
        catch (Exception ex)
        {
            _logger.Here().Error(ex, "Error creating cluster with ID {ClusterId}", cluster.ClusterId);
            throw;
        }
    }

    public async Task<ProxyCluster> GetByClusterIdAsync(string clusterId, CancellationToken cancellationToken = default)
    {
        _logger.Here().MethodEntered();
        
        try
        {
            var cluster = await _repository.Include(GetDefaultIncludes())
                .FirstOrDefaultAsync(c => c.ClusterId == clusterId, cancellationToken);

            if (cluster == null)
            {
                _logger.Here().Warning("Cluster with ClusterId {ClusterId} not found", clusterId);
                throw new InvalidOperationException($"Cluster with ClusterId {clusterId} not found");
            }

            _logger.Here().MethodExited(cluster);
            return cluster;
        }
        catch (Exception ex)
        {
            _logger.Here().Error(ex, "Error getting cluster by ClusterId {ClusterId}", clusterId);
            throw;
        }
    }

    protected override Expression<Func<ProxyCluster, object>>[] GetDefaultIncludes()
    {
        return
        [
            c => c.Destinations
        ];
    }
} 
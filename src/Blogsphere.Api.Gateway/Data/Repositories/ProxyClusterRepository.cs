using Blogsphere.Api.Gateway.Data.Interfaces.Repositories;
using Blogsphere.Api.Gateway.Data.Repositories.Base;
using Blogsphere.Api.Gateway.Entity;
using Blogsphere.Api.Gateway.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Blogsphere.Api.Gateway.Data.Repositories;

public class ProxyClusterRepository(ProxyConfigContext context) : Repository<ProxyCluster>(context), IProxyClusterRepository
{
    private readonly ProxyConfigContext _context = context;

    public async Task<ProxyCluster> GetByClusterIdAsync(string clusterId, CancellationToken cancellationToken = default)
    {
        return await Include(c => c.Destinations)
                .SingleOrDefaultAsync(c => c.ClusterId == clusterId, cancellationToken);
    }

    public async Task<IEnumerable<ProxyCluster>> GetActiveWithDestinationsAsync(CancellationToken cancellationToken = default)
    {
        return await Include(c => c.Destinations)
                .Where(c => c.IsActive && c.Destinations.Any(d => d.IsActive))
                .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasActiveDestinationsAsync(string clusterId, CancellationToken cancellationToken = default)
    {
        return await _context.Destinations
                .AnyAsync(d => d.Cluster.ClusterId == clusterId && d.IsActive, cancellationToken);
    }
} 
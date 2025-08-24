using Microsoft.EntityFrameworkCore;

namespace Blogsphere.Api.Gateway.Data.Repositories;

public class ProxyDestinationRepository(ProxyConfigContext context) : Repository<ProxyDestination>(context), IProxyDestinationRepository
{
    private readonly ProxyConfigContext _context = context;

    public async Task<ProxyDestination> GetByDestinationIdAsync(string destinationId, CancellationToken cancellationToken = default)
    {
        return await Include(d => d.Cluster)
                .SingleOrDefaultAsync(d => d.DestinationId == destinationId, cancellationToken);
    }

    public async Task<IEnumerable<ProxyDestination>> GetByClusterIdAsync(Guid clusterId, CancellationToken cancellationToken = default)
    {
        return await Include(d => d.Cluster)
                .Where(d => d.ClusterId == clusterId)
                .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProxyDestination>> GetActiveDestinationsAsync(Guid clusterId, CancellationToken cancellationToken = default)
    {
        return await Include(d => d.Cluster)
                .Where(d => d.ClusterId == clusterId && d.IsActive && d.Cluster.IsActive)
                .ToListAsync(cancellationToken);
    }
} 
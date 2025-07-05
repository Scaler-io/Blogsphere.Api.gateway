using Blogsphere.Api.Gateway.Data.Interfaces.Repositories;
using Blogsphere.Api.Gateway.Data.Repositories.Base;
using Blogsphere.Api.Gateway.Entity;
using Blogsphere.Api.Gateway.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Blogsphere.Api.Gateway.Data.Repositories;

public class ProxyRouteRepository(ProxyConfigContext context) : Repository<ProxyRoute>(context), IProxyRouteRepository
{
    private readonly ProxyConfigContext _context = context;

    public async Task<ProxyRoute> GetByRouteIdAsync(string routeId, CancellationToken cancellationToken = default)
    {
        return await Include(r => r.Headers, r => r.Transforms)
                .SingleOrDefaultAsync(r => r.RouteId == routeId, cancellationToken);
    }

    public async Task<IEnumerable<ProxyRoute>> GetByClusterIdAsync(Guid clusterId, CancellationToken cancellationToken = default)
    {
        return await Include(r => r.Headers, r => r.Transforms)
                .Where(r => r.ClusterId == clusterId)
                .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProxyRoute>> GetActiveRoutesAsync(CancellationToken cancellationToken = default)
    {
        return await Include(r => r.Headers, r => r.Transforms)
                .Where(r => r.IsActive)
                .ToListAsync(cancellationToken);
    }
} 
using Blogsphere.Api.Gateway.Data.Interfaces.Repositories;
using Blogsphere.Api.Gateway.Data.Repositories.Base;
using Blogsphere.Api.Gateway.Entity;
using Blogsphere.Api.Gateway.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Blogsphere.Api.Gateway.Data.Repositories;

public class ProxyTransformRepository(ProxyConfigContext context) : Repository<ProxyTransform>(context), IProxyTransformRepository
{
    private readonly ProxyConfigContext _context = context;

    public async Task<IEnumerable<ProxyTransform>> GetByRouteIdAsync(Guid routeId, CancellationToken cancellationToken = default)
    {
        return await Include(t => t.Route)
                .Where(t => t.RouteId == routeId)
                .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProxyTransform>> GetByPathPatternAsync(string pathPattern, CancellationToken cancellationToken = default)
    {
        return await Include(t => t.Route)
                .Where(t => t.PathPattern == pathPattern)
                .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsForRouteAsync(Guid routeId, string pathPattern, CancellationToken cancellationToken = default)
    {
        return await _context.Transforms
                .AnyAsync(t => t.RouteId == routeId && t.PathPattern == pathPattern, cancellationToken);
    }
} 
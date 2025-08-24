using Microsoft.EntityFrameworkCore;

namespace Blogsphere.Api.Gateway.Data.Repositories;

public class ProxyHeaderRepository(ProxyConfigContext context) : Repository<ProxyHeader>(context), IProxyHeaderRepository
{
    private readonly ProxyConfigContext _context = context;

    public async Task<IEnumerable<ProxyHeader>> GetByRouteIdAsync(Guid routeId, CancellationToken cancellationToken = default)
    {
        return await Include(h => h.Route)
                .Where(h => h.RouteId == routeId)
                .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProxyHeader>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Include(h => h.Route)
                .Where(h => h.Name == name)
                .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsForRouteAsync(Guid routeId, string name, CancellationToken cancellationToken = default)
    {
        return await _context.Headers
                .AnyAsync(h => h.RouteId == routeId && h.Name == name, cancellationToken);
    }
} 
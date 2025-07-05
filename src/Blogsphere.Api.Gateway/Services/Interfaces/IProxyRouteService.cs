using Blogsphere.Api.Gateway.Entity;

namespace Blogsphere.Api.Gateway.Services.Interfaces;

public interface IProxyRouteService
{
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    Task<ProxyRoute> GetByRouteIdAsync(string routeId, CancellationToken cancellationToken = default);
    Task<ProxyRoute> CreateAsync(ProxyRoute route, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProxyRoute>> GetAllAsync(CancellationToken cancellationToken = default);
} 
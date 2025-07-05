using Blogsphere.Api.Gateway.Data.Interfaces.Base;
using Blogsphere.Api.Gateway.Entity;

namespace Blogsphere.Api.Gateway.Data.Interfaces.Repositories;

public interface IProxyRouteRepository : IRepository<ProxyRoute>
{
    Task<ProxyRoute> GetByRouteIdAsync(string routeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProxyRoute>> GetByClusterIdAsync(Guid clusterId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProxyRoute>> GetActiveRoutesAsync(CancellationToken cancellationToken = default);
} 
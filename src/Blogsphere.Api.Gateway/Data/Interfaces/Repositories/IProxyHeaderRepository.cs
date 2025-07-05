using Blogsphere.Api.Gateway.Data.Interfaces.Base;
using Blogsphere.Api.Gateway.Entity;

namespace Blogsphere.Api.Gateway.Data.Interfaces.Repositories;

public interface IProxyHeaderRepository : IRepository<ProxyHeader>
{
    Task<IEnumerable<ProxyHeader>> GetByRouteIdAsync(Guid routeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProxyHeader>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> ExistsForRouteAsync(Guid routeId, string name, CancellationToken cancellationToken = default);
} 
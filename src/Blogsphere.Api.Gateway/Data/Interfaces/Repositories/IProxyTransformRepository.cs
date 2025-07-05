using Blogsphere.Api.Gateway.Data.Interfaces.Base;
using Blogsphere.Api.Gateway.Entity;

namespace Blogsphere.Api.Gateway.Data.Interfaces.Repositories;

public interface IProxyTransformRepository : IRepository<ProxyTransform>
{
    Task<IEnumerable<ProxyTransform>> GetByRouteIdAsync(Guid routeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProxyTransform>> GetByPathPatternAsync(string pathPattern, CancellationToken cancellationToken = default);
    Task<bool> ExistsForRouteAsync(Guid routeId, string pathPattern, CancellationToken cancellationToken = default);
} 
using Blogsphere.Api.Gateway.Entity;

namespace Blogsphere.Api.Gateway.Services.Interfaces;

public interface IProxyClusterService
{
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    Task<ProxyCluster> GetByClusterIdAsync(string clusterId, CancellationToken cancellationToken = default);
    Task<ProxyCluster> CreateAsync(ProxyCluster cluster, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProxyCluster>> GetAllAsync(CancellationToken cancellationToken = default);
} 
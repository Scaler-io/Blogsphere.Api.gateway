namespace Blogsphere.Api.Gateway.Data.Interfaces.Repositories;

public interface IProxyClusterRepository : IRepository<ProxyCluster>
{
    Task<ProxyCluster> GetByClusterIdAsync(string clusterId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProxyCluster>> GetActiveWithDestinationsAsync(CancellationToken cancellationToken = default);
    Task<bool> HasActiveDestinationsAsync(string clusterId, CancellationToken cancellationToken = default);
} 
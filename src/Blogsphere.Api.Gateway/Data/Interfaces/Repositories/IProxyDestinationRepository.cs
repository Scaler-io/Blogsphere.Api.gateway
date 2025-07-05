using Blogsphere.Api.Gateway.Data.Interfaces.Base;
using Blogsphere.Api.Gateway.Entity;

namespace Blogsphere.Api.Gateway.Data.Interfaces.Repositories;

public interface IProxyDestinationRepository : IRepository<ProxyDestination>
{
    Task<ProxyDestination> GetByDestinationIdAsync(string destinationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProxyDestination>> GetByClusterIdAsync(Guid clusterId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProxyDestination>> GetActiveDestinationsAsync(Guid clusterId, CancellationToken cancellationToken = default);
} 
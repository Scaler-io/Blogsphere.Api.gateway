using Blogsphere.Api.Gateway.Entity;

namespace Blogsphere.Api.Gateway.Services.Interfaces;

public interface IProxyDestinationService
{
    Task<ProxyDestination> GetByDestinationIdAsync(string destinationId, CancellationToken cancellationToken = default);
    Task<ProxyDestination> CreateAsync(ProxyDestination destination, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProxyDestination>> GetAllAsync(CancellationToken cancellationToken = default);
} 
namespace Blogsphere.Api.Gateway.Data.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IProxyRouteRepository Routes { get; }
    IProxyClusterRepository Clusters { get; }
    IProxyDestinationRepository Destinations { get; }
    IProxyHeaderRepository Headers { get; }
    IProxyTransformRepository Transforms { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
} 
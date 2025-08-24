using Microsoft.EntityFrameworkCore.Storage;

namespace Blogsphere.Api.Gateway.Data.UnitOfWork;

public class UnitOfWork(
    ProxyConfigContext context,
    IProxyRouteRepository routes,
    IProxyClusterRepository clusters,
    IProxyDestinationRepository destinations,
    IProxyHeaderRepository headers,
    IProxyTransformRepository transforms) : IUnitOfWork
{
    private readonly ProxyConfigContext _context = context;
    private IDbContextTransaction _transaction;
    private bool _disposed;

    public IProxyRouteRepository Routes { get; } = routes;
    public IProxyClusterRepository Clusters { get; } = clusters;
    public IProxyDestinationRepository Destinations { get; } = destinations;
    public IProxyHeaderRepository Headers { get; } = headers;
    public IProxyTransformRepository Transforms { get; } = transforms;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("Transaction already started");
        }

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _transaction?.CommitAsync(cancellationToken)!;
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context.Dispose();
            _transaction?.Dispose();
        }
        _disposed = true;
    }
} 
using System.Linq.Expressions;
using Blogsphere.Api.Gateway.Data.Interfaces;
using Blogsphere.Api.Gateway.Data.Interfaces.Repositories;
using Blogsphere.Api.Gateway.Entity;
using Blogsphere.Api.Gateway.Extensions;
using Blogsphere.Api.Gateway.Services.Base;
using Blogsphere.Api.Gateway.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blogsphere.Api.Gateway.Services;

public class ProxyRouteService(
    ILogger logger,
    IProxyRouteRepository repository,
    IUnitOfWork unitOfWork) : BaseService<ProxyRoute>(logger, repository, unitOfWork), IProxyRouteService
{
    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        _logger.Here().MethodEntered();
        
        try
        {
            var result = await _repository.AnyAsync(x => true, cancellationToken);
            _logger.Here().MethodExited(result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.Here().Error(ex, "Error checking if any routes exist");
            throw;
        }
    }

    public async Task<IEnumerable<ProxyRoute>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.Here().MethodEntered();
        
        try
        {
            var routes = await _repository.Include(GetDefaultIncludes())
                .ToListAsync(cancellationToken);

            _logger.Here().Information("Retrieved {Count} routes", routes.Count);
            _logger.Here().MethodExited(routes);
            return routes;
        }
        catch (Exception ex)
        {
            _logger.Here().Error(ex, "Error getting all routes");
            throw;
        }
    }

    public async Task<ProxyRoute> CreateAsync(ProxyRoute route, CancellationToken cancellationToken = default)
    {
        _logger.Here().MethodEntered();
        
        try
        {
            await _repository.AddAsync(route, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.Here().Information("Created route with ID {RouteId}", route.RouteId);
            _logger.Here().MethodExited(route);
            return route;
        }
        catch (Exception ex)
        {
            _logger.Here().Error(ex, "Error creating route with ID {RouteId}", route.RouteId);
            throw;
        }
    }

    public async Task<ProxyRoute> GetByRouteIdAsync(string routeId, CancellationToken cancellationToken = default)
    {
        _logger.Here().MethodEntered();
        
        try
        {
            var route = await _repository.Include(GetDefaultIncludes())
                .FirstOrDefaultAsync(r => r.RouteId == routeId, cancellationToken);

            if (route == null)
            {
                _logger.Here().Warning("Route with ID {RouteId} not found", routeId);
                throw new InvalidOperationException($"Route with ID {routeId} not found");
            }

            _logger.Here().MethodExited(route);
            return route;
        }
        catch (Exception ex)
        {
            _logger.Here().Error(ex, "Error getting route by ID {RouteId}", routeId);
            throw;
        }
    }

    protected override Expression<Func<ProxyRoute, object>>[] GetDefaultIncludes()
    {
        return
        [
            r => r.Headers,
            r => r.Transforms,
            r => r.Cluster
        ];
    }
} 
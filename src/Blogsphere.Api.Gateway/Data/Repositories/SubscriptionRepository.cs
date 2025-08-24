
using Microsoft.EntityFrameworkCore;

namespace Blogsphere.Api.Gateway.Data.Repositories;

public class SubscriptionRepository<T>(ProxyConfigContext context)
: Repository<T>(context), ISubscriptionRepository<T> where T : EntityBase
{
    public async Task<bool> IsApiProductNameUniqueAsync(string productName)
    {
        var c = await Context.Set<ApiProduct>()
         .AnyAsync(e => e.ProductName == productName);
        return c;
    }     
    public async Task<bool> IsSubscribedApiNameUniqueAsync(string apiName)
    {
        var c = await Context.Set<SubscribedApi>()
         .AnyAsync(e => e.ApiName == apiName);
        return c;
    }
    public async Task<bool> IsSubscribedApiPathUniqueAsync(string apiPath)
    {
        var c = await Context.Set<SubscribedApi>()
         .AnyAsync(e => e.ApiPath == apiPath);
        return c;
    }       
}

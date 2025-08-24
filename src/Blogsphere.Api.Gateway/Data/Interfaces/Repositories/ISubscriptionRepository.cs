namespace Blogsphere.Api.Gateway.Data.Interfaces.Repositories;

public interface ISubscriptionRepository<T> : IRepository<T> where T : EntityBase
{
    Task<bool> IsApiProductNameUniqueAsync(string productName);
    Task<bool> IsSubscribedApiNameUniqueAsync(string apiName);
    Task<bool> IsSubscribedApiPathUniqueAsync(string apiPath);
}

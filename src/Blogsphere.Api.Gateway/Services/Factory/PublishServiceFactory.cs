namespace Blogsphere.Api.Gateway.Services.Factory;

public class PublishServiceFactory(IServiceProvider serviceProvider) : IPublishServiceFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public IPublishService<T, TEvent> CreatePublishServiceAsync<T, TEvent>() where T : EntityBase where TEvent : GenericEvent
    {
        return _serviceProvider.GetRequiredService<IPublishService<T, TEvent>>();
    }
}

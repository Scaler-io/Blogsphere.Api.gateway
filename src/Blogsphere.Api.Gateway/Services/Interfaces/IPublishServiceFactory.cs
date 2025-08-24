namespace Blogsphere.Api.Gateway.Services.Interfaces;

public interface IPublishServiceFactory
{
    IPublishService<T, TEvent> CreatePublishServiceAsync<T, TEvent>()
        where T : EntityBase 
        where TEvent : GenericEvent;
}

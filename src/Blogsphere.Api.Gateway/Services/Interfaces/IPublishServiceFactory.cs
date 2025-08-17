using Blogsphere.Api.Gateway.Entity;
using Blogsphere.Api.Gateway.EventBus;

namespace Blogsphere.Api.Gateway.Services.Interfaces;

public interface IPublishServiceFactory
{
    IPublishService<T, TEvent> CreatePublishServiceAsync<T, TEvent>()
        where T : EntityBase 
        where TEvent : GenericEvent;
}

namespace Blogsphere.Api.Gateway.Services.Interfaces;

public interface IPublishService<T, TEvent> where T : EntityBase where TEvent : GenericEvent
{
    Task PublishAsync(T message, string correlationId, object additionalProperties = null);
}

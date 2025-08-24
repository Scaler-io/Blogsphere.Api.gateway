using MassTransit;
using AutoMapper;

namespace Blogsphere.Api.Gateway.Services;

public class PublishService<T, TEvent>(
    IPublishEndpoint publishEndpoint,
    ILogger logger,
    IMapper mapper) : IPublishService<T, TEvent> where T : EntityBase where TEvent : GenericEvent
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    private readonly ILogger _logger = logger;
    private readonly IMapper _mapper = mapper;

    public async Task PublishAsync(T message, string correlationId, object additionalProperties = null)
    {
        var newEvent = _mapper.Map<TEvent>(message);
        newEvent.CorrelationId = correlationId;
        newEvent.AdditionalProperties = additionalProperties;

        await _publishEndpoint.Publish(newEvent);

        _logger.Here().WithCorrelationId(correlationId).Information("Successfully published {messageType} event message", typeof(TEvent).Name);
    }
}

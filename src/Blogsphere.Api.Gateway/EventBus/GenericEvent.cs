using Blogsphere.Api.Gateway.Models.Enums;

namespace Blogsphere.Api.Gateway.EventBus;

public abstract class GenericEvent
{
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public string CorrelationId { get; set; }
    public object AdditionalProperties { get; set; }
    protected abstract GenericEventType Type { get; set; }
}

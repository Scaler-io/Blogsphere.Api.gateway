namespace Blogsphere.Api.Gateway.Entity;

public class ProxyTransform : EntityBase
{
    public string PathPattern { get; set; }
    public Guid RouteId { get; set; }
    public ProxyRoute Route { get; set; }
} 
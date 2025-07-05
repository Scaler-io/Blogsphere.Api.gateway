namespace Blogsphere.Api.Gateway.Entity;

public class ProxyHeader : EntityBase
{
    public string Name { get; set; }
    public string[] Values { get; set; }
    public string Mode { get; set; }
    public Guid RouteId { get; set; }
    public ProxyRoute Route { get; set; }
} 
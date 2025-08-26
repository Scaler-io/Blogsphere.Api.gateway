namespace Blogsphere.Api.Gateway.Entity;

public class ProxyRoute : EntityBase
{
    public string RouteId { get; set; }
    public string Path { get; set; }
    public string[] Methods { get; set; }
    public string RateLimiterPolicy { get; set; }
    public Guid ClusterId { get; set; }
    public ProxyCluster Cluster { get; set; }
    public ICollection<ProxyHeader> Headers { get; set; } = [];
    public ICollection<ProxyTransform> Transforms { get; set; } = [];
} 
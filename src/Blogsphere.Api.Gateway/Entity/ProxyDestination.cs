namespace Blogsphere.Api.Gateway.Entity;

public class ProxyDestination : EntityBase
{
    public string DestinationId { get; set; }
    public string Address { get; set; }
    public Guid ClusterId { get; set; }
    public ProxyCluster Cluster { get; set; }
} 
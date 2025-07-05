namespace Blogsphere.Api.Gateway.Entity;

public class ProxyCluster : EntityBase
{
    public string ClusterId { get; set; }
    public string LoadBalancingPolicy { get; set; }
    public bool HealthCheckEnabled { get; set; }
    public string HealthCheckPath { get; set; }
    public int HealthCheckInterval { get; set; }
    public int HealthCheckTimeout { get; set; }
    public ICollection<ProxyRoute> Routes { get; set; } = [];
    public ICollection<ProxyDestination> Destinations { get; set; } = [];
} 
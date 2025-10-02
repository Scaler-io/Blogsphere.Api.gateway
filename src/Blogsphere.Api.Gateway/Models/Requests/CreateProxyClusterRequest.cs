public class CreateProxyClusterRequest
{
    public string ClusterId { get; set; }
    
    public string LoadBalancingPolicy { get; set; }
    
    public bool IsActive { get; set; } = true;
    public bool HealthCheckEnabled { get; set; }
    
    public string HealthCheckPath { get; set; }
    
    public int? HealthCheckInterval { get; set; }
    
    public int? HealthCheckTimeout { get; set; }
    
    public ICollection<CreateProxyDestinationRequest> Destinations { get; set; }
} 
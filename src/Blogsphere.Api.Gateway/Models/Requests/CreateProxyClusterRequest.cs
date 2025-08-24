public class CreateProxyClusterRequest
{
    public string ClusterId { get; set; }
    
    public string LoadBalancingPolicy { get; set; } = "RoundRobin";
    
    public bool HealthCheckEnabled { get; set; } = true;
    
    public string HealthCheckPath { get; set; } = "/health";
    
    public int HealthCheckInterval { get; set; } = 30;
    
    public int HealthCheckTimeout { get; set; } = 10;
    
    public ICollection<CreateProxyDestinationRequest> Destinations { get; set; }
} 
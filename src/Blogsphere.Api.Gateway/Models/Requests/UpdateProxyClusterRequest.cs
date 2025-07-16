using System.ComponentModel.DataAnnotations;

namespace Blogsphere.Api.Gateway.Models.Requests;

public class UpdateProxyClusterRequest
{
    public string ClusterId { get; set; }
    
    public string LoadBalancingPolicy { get; set; }
    
    public bool? HealthCheckEnabled { get; set; }
    
    public string HealthCheckPath { get; set; }
    
    public int? HealthCheckInterval { get; set; }
    
    public int? HealthCheckTimeout { get; set; }
    
    public ICollection<UpdateProxyDestinationRequest> Destinations { get; set; }
    
    /// <summary>
    /// List of destination IDs to remove from the cluster
    /// </summary>
    public ICollection<string> RemoveDestinations { get; set; }
} 
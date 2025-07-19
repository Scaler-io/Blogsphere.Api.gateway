namespace Blogsphere.Api.Gateway.Models.DTOs;

public class ProxyClusterDto
{
    public Guid Id { get; set; }
    public string ClusterId { get; set; }
    public string LoadBalancingPolicy { get; set; }
    public bool HealthCheckEnabled { get; set; }
    public string HealthCheckPath { get; set; }
    public int HealthCheckInterval { get; set; }
    public int HealthCheckTimeout { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
    public ICollection<ProxyDestinationDto> Destinations { get; set; } = [];
    public ICollection<ProxyRouteDto> Routes { get; set; } = [];
} 
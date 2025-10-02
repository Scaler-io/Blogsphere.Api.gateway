using System.ComponentModel.DataAnnotations;

namespace Blogsphere.Api.Gateway.Models.Requests;

public class CreateProxyRouteRequest
{
    [Required]
    public string RouteId { get; set; }
    
    [Required]
    public string Path { get; set; }
    
    [Required]
    public string[] Methods { get; set; }
    
    public string RateLimiterPolicy { get; set; }
    public bool IsActive { get; set; }
    
    public Dictionary<string, string> Metadata { get; set; }
    
    [Required]
    public Guid ClusterId { get; set; }
    
    public ICollection<ProxyHeaderRequest> Headers { get; set; }
    
    public ICollection<ProxyTransformRequest> Transforms { get; set; }
}

public class ProxyHeaderRequest
{
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string[] Values { get; set; }
    
    public string Mode { get; set; } = "Set";
    public bool IsActive { get; set; }
}

public class ProxyTransformRequest
{
    [Required]
    public string PathPattern { get; set; }
    public bool IsActive { get; set; }
} 
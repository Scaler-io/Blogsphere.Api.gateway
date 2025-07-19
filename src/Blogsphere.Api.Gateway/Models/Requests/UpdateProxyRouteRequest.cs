namespace Blogsphere.Api.Gateway.Models.Requests;

public class UpdateProxyRouteRequest
{
    public string RouteId { get; set; }
    
    public string Path { get; set; }
    
    public string[] Methods { get; set; }
    
    public string RateLimiterPolicy { get; set; }
    
    public Dictionary<string, string> Metadata { get; set; }
    
    public Guid? ClusterId { get; set; }
    
    public ICollection<ProxyHeaderRequest> Headers { get; set; }
    
    public ICollection<ProxyTransformRequest> Transforms { get; set; }
    
    /// <summary>
    /// List of header names to remove from the route
    /// </summary>
    public ICollection<string> RemoveHeaders { get; set; }
    
    /// <summary>
    /// List of transform path patterns to remove from the route
    /// </summary>
    public ICollection<string> RemoveTransforms { get; set; }
} 
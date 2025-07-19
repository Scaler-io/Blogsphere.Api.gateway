namespace Blogsphere.Api.Gateway.Models.DTOs;

public class ProxyRouteDto
{
    public Guid Id { get; set; }
    public string RouteId { get; set; }
    public string Path { get; set; }
    public string[] Methods { get; set; }
    public string RateLimiterPolicy { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
    public bool IsActive { get; set; }
    public Guid ClusterId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
    public ICollection<ProxyHeaderDto> Headers { get; set; } = [];
    public ICollection<ProxyTransformDto> Transforms { get; set; } = [];
}

public class ProxyHeaderDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string[] Values { get; set; }
    public string Mode { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
}

public class ProxyTransformDto
{
    public Guid Id { get; set; }
    public string PathPattern { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
} 
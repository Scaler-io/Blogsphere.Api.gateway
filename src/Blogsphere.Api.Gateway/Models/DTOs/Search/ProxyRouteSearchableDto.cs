namespace Blogsphere.Api.Gateway.Models.DTOs.Search;

public class ProxyRouteSearchableDto
{
    public string Id { get; set; }
    public string RouteId { get; set; }
    public string Path { get; set; }
    public string Cluster { get; set; }
    public string RateLimitterPolicy { get; set; }
    public long TransformCount { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

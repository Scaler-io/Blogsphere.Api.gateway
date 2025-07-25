namespace Blogsphere.Api.Gateway.Models.DTOs;

public class ProxyDestinationDto
{
    public Guid Id { get; set; }
    public string DestinationId { get; set; }
    public string Address { get; set; }
    public bool IsActive { get; set; }
    public Guid ClusterId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
} 
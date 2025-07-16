using System.ComponentModel.DataAnnotations;

namespace Blogsphere.Api.Gateway.Models.Requests;

public class CreateProxyDestinationRequest
{
    [Required]
    public string DestinationId { get; set; }
    
    [Required]
    [Url]
    public string Address { get; set; }
    
    public bool IsActive { get; set; } = true;
} 
using System.ComponentModel.DataAnnotations;

namespace Blogsphere.Api.Gateway.Models.Requests;

public class UpdateProxyDestinationRequest
{
    [Required]
    public string DestinationId { get; set; }
    
    [Url]
    public string Address { get; set; }
    
    public bool? IsActive { get; set; }
} 
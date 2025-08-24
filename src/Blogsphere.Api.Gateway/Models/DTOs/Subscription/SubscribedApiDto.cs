namespace Blogsphere.Api.Gateway.Models.DTOs.Subscription;

public class SubscribedApiDto
{
    public Guid Id { get; set; }
    public string ApiPath { get; set; }
    public string ApiName { get; set; }
    public string ApiDescription { get; set; }
    public ApiProductSummary ApiProductDetails { get; set; }
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }    
}


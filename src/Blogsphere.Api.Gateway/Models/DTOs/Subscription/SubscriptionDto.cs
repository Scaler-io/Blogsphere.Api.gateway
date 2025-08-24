namespace Blogsphere.Api.Gateway.Models.DTOs.Subscription;

public class SubscriptionDto
{
    public Guid Id { get; set; }
    public string SubscriptionName { get; set; }
    public string SubscriptionDescription { get; set; }
    public ApiProductSummary ApiProductDetails { get; set; }
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
}
namespace Blogsphere.Api.Gateway.Models.DTOs.Subscription;

public class ApiProductDto
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductDescription { get; set; }
    public bool IsActive { get; set; }
    public int SubscribedApiCount { get; set; }
    public int SubscriptionCount { get; set; }
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
}

public class ApiProductSummary
{
    public Guid Id { get; set; }
    public string ProductName { get; set; }
}
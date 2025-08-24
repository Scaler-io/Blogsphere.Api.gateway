namespace Blogsphere.Api.Gateway.Entity;

public class SubscribedApi : EntityBase
{
    public string ApiPath { get; set; }
    public string ApiName { get; set; }
    public string ApiDescription { get; set; }
    public Guid ProductId { get; set; }
    public ApiProduct ApiProduct { get; set; }
}

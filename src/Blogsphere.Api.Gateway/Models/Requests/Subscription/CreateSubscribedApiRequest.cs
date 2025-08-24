namespace Blogsphere.Api.Gateway.Models.Requests.Subscription;

public class CreateSubscribedApiRequest
{
    public string ApiPath { get; set; }
    public string ApiName { get; set; }
    public string ApiDescription { get; set; }
    public Guid ProductId { get; set; }
}

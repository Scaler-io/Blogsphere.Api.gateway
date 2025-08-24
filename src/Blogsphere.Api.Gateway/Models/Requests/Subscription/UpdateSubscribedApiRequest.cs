namespace Blogsphere.Api.Gateway.Models.Requests.Subscription;

public class UpdateSubscribedApiRequest
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }    
}

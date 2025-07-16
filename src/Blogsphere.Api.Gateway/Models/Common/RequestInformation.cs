namespace Blogsphere.Api.Gateway.Models.Common;

public class RequestInformation
{
    public string CorrelationId { get; set; }
    public UserDto CurrentUser { get; set; }
}

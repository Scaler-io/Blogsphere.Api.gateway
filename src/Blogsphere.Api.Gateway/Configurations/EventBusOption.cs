namespace Blogsphere.Api.Gateway.Configurations;

public class EventBusOption
{
    public const string OptionName = "EventBus";
    public string Host { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string VirtualHost { get; set; }
}

namespace Blogsphere.Api.Gateway.Configurations;

public class AppConfigOption
{
    public static string OptionName = "AppConfigurations";
    public string ApplicationIdentifier { get; set; }
    public string ApplicationEnvironment { get; set; }
}

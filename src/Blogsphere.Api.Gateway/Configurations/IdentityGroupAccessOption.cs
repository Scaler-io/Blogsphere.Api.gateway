namespace Blogsphere.Api.Gateway.Configurations;

public class IdentityGroupAccessOption
{
    public static string OptionName = "IdentityGroupAccess";
    public string Authority { get; set; }
    public string Audience { get; set; }
}

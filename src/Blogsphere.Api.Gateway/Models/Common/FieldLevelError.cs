namespace Blogsphere.Api.Gateway.Models.Common;

public class FieldLevelError
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; }
    public string Field { get; set; }
}

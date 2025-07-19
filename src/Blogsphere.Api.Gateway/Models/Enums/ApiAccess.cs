using System.Runtime.Serialization;

namespace Blogsphere.Api.Gateway.Models.Enums;

public enum ApiAccess
{
    [EnumMember(Value = "system:view-settings")]
    CanViewSystemSettings,
    [EnumMember(Value = "system:update-settings")]
    CanUpdateSystemSettings,
}

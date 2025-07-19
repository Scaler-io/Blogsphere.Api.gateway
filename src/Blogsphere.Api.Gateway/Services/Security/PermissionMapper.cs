using Blogsphere.Api.Gateway.Extensions;
using Blogsphere.Api.Gateway.Models.Enums;
using Blogsphere.Api.Gateway.Services.Interfaces;

namespace Blogsphere.Api.Gateway.Services.Security;

public class PermissionMapper : IPermissionMapper
{
    public readonly Dictionary<ApiAccess, List<string>> _map = new()
    {
        {ApiAccess.CanViewSystemSettings, [ApiAccess.CanViewSystemSettings.GetEnumMemberValue()]},
        {ApiAccess.CanUpdateSystemSettings, [ApiAccess.CanUpdateSystemSettings.GetEnumMemberValue()]},
    };
    
    public List<string> GetPermissionsForRole(ApiAccess role)
    {
        return _map.TryGetValue(role, out var permissions) ? permissions : [];
    }
}

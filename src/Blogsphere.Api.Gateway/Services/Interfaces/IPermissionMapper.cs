using Blogsphere.Api.Gateway.Models.Enums;

namespace Blogsphere.Api.Gateway.Services.Interfaces;

public interface IPermissionMapper
{
    List<string> GetPermissionsForRole(ApiAccess role);
}

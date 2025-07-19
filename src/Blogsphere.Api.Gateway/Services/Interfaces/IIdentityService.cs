using Blogsphere.Api.Gateway.Models.Common;

namespace Blogsphere.Api.Gateway.Services.Interfaces;

public interface IIdentityService
{
    UserDto PrepareUser();
}

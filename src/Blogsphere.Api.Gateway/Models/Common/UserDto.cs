namespace Blogsphere.Api.Gateway.Models.Common;

public class UserDto
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public AuthorizationDto AuthorizationDto { get; set; }

    public bool IsAdmin()
    {
        return AuthorizationDto.Roles.Contains("Admin");
    }
}

public class AuthorizationDto
{
    public IList<string> Roles { get; set; }
    public IList<string> Permissions { get; set; }
    public string Token { get; set; }
}

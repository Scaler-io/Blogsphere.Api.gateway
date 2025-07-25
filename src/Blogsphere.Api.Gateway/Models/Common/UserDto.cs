namespace Blogsphere.Api.Gateway.Models.Common;

public class UserDto
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public AuthorizationDto AuthorizationDto { get; set; }

    public bool IsAdmin()
    {
        return AuthorizationDto.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
    }
}

public class AuthorizationDto
{
    public string Role { get; set; }
    public IList<string> Permissions { get; set; }
    public string Token { get; set; }
}

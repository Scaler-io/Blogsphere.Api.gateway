using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Blogsphere.Api.Gateway.Services.Security;

public class IdentityService(IHttpContextAccessor contextAccessor) : IIdentityService
{
    private readonly IHttpContextAccessor _contextAccessor = contextAccessor;

    public const string IdClaim = ClaimTypes.NameIdentifier;
    public const string RoleClaim = ClaimTypes.Role;
    public const string FirstNameClaim = ClaimTypes.GivenName;
    public const string LastNameClaim = ClaimTypes.Surname;
    public const string NameClaim = JwtRegisteredClaimNames.Name;
    public const string EmailClaim = ClaimTypes.Email;
    public const string PermissionClaim = "permissions";

    public UserDto PrepareUser()
    {
        var token = _contextAccessor.HttpContext.Request.Headers.Authorization;
        var claims = _contextAccessor.HttpContext.User.Claims;

        if (claims.IsNullOrEmpty())
        {
            return null;
        }

        var permissionsString = claims.Where(c => c.Type == PermissionClaim).FirstOrDefault()?.Value;
        var id = claims.Where(c => c.Type == IdClaim).FirstOrDefault().Value;
        var firstName = claims.Where(c => c.Type == FirstNameClaim).FirstOrDefault().Value;
        var lastName = claims.Where(c => c.Type == LastNameClaim).FirstOrDefault().Value;
        var name = claims.Where(c => c.Type == NameClaim).FirstOrDefault().Value;
        var email = claims.Where(c => c.Type == EmailClaim).FirstOrDefault().Value;
        var role = claims.Where(c => c.Type == RoleClaim).FirstOrDefault()?.Value;
        var permissions = permissionsString == "*" ? ["*"] : JsonConvert.DeserializeObject<List<string>>(permissionsString);

        return new UserDto
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            Name = name,
            Email = email,
            AuthorizationDto = new AuthorizationDto
            {
                Role = role,
                Permissions = permissions,
                Token = token
            }
        };
    }
}

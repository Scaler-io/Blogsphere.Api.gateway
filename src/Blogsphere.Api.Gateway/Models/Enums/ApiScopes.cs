using System.Runtime.Serialization;

namespace Blogsphere.Api.Gateway.Models.Enums;

public static class ApiScopes
{
    public const string GatewayRead = "apigateway:read";
    public const string GatewayWrite = "apigateway:write";
    public const string GatewayDelete = "apigateway:delete";
}

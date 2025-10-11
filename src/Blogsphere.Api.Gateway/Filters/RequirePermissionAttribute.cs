using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Blogsphere.Api.Gateway.Filters;

public class RequirePermissionAttribute : TypeFilterAttribute
{
    public RequirePermissionAttribute(ApiAccess permission, params string[] scopes) : base(typeof(RequirePermissionExecutor))
    {
        Arguments = [permission, scopes];
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequirePermissionExecutor(IIdentityService identityService, ILogger logger, IPermissionMapper permissionMapper, ApiAccess permission, params string[] scopes) : Attribute, IActionFilter
    {
        private readonly IIdentityService _identityService = identityService;
        private readonly ILogger _logger = logger;
        private readonly ApiAccess _permission = permission;
        private readonly IPermissionMapper _permissionMapper = permissionMapper;
        private readonly List<string> _scopes = [.. scopes];

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.Here().MethodEntered();
            if (_scopes.Any())
            {
                var availableScopes = context.HttpContext
                    .User
                    .Claims
                    .Where(c => c.Type == "scope")
                    .Select(c => c.Value)
                    .ToList();

                if(_scopes.Any(s => !availableScopes.Contains(s)))
                {
                    _logger.Here().Error("No matching scope found");
                    context.Result = new UnauthorizedObjectResult(new ApiResponse(ErrorCodes.Unauthorized, "Access denied"));
                    return;
                }
            }

            var isM2MRequest = context.HttpContext.Request.Headers.TryGetValue("X-M2M-Request", out var m2mRequest);
            if (isM2MRequest)
            {
                _logger.Here().Debug("M2M request detected, skipping permission check");
                return;
            }
            
            var currentUser = _identityService.PrepareUser();
            if(currentUser.AuthorizationDto.Permissions.Contains("*"))
            {
                _logger.Here().Debug("All permissions are allowed, skipping permission check");
                return;
            }

            // Alternative optimization for larger permission sets:
            // Convert to HashSet for O(1) lookups instead of O(n) Contains operations
            var requiredPermissions = _permissionMapper.GetPermissionsForRole(_permission);
            var requiredPermissionsSet = requiredPermissions.ToHashSet();

            // Short-circuit evaluation: stops as soon as first match is found
            var hasRequiredPermission = currentUser.AuthorizationDto.Permissions
                .Any(requiredPermissionsSet.Contains);


            if (!hasRequiredPermission)
            {
                _logger.Here().Error("No matching permission found");
                context.Result = new UnauthorizedObjectResult(new ApiResponse(ErrorCodes.Unauthorized, "Access denied"));
            }

            _logger.Here().MethodExited();
        }
    }
}

using Blogsphere.Api.Gateway.Extensions;
using Blogsphere.Api.Gateway.Models.Common;
using Blogsphere.Api.Gateway.Models.Enums;
using Blogsphere.Api.Gateway.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Blogsphere.Api.Gateway.Filters;

public class RequirePermissionAttribute : TypeFilterAttribute
{
    public RequirePermissionAttribute(ApiAccess permission) : base(typeof(RequirePermissionExecutor))
    {
        Arguments = [permission];
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequirePermissionExecutor(IIdentityService identityService, ILogger logger, IPermissionMapper permissionMapper, ApiAccess permission) : Attribute, IActionFilter
    {
        private readonly IIdentityService _identityService = identityService;
        private readonly ILogger _logger = logger;
        private readonly ApiAccess _permission = permission;
        private readonly IPermissionMapper _permissionMapper = permissionMapper;

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.Here().MethodEntered();

            var isM2MRequest = context.HttpContext.Request.Headers.TryGetValue("X-M2M-Request", out var m2mRequest);
            if (isM2MRequest)
            {
                _logger.Here().Debug("M2M request detected, skipping permission check");
                return;
            }
            
            var currentUser = _identityService.PrepareUser();

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

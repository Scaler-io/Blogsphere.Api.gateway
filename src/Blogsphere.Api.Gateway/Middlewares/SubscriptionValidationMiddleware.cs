using Blogsphere.Api.Gateway.Services;
using Blogsphere.Api.Gateway.Models.Common;

namespace Blogsphere.Api.Gateway.Middlewares;

public class SubscriptionValidationMiddleware(ILogger logger, ISubscriptionService validator) : IMiddleware
{
    private readonly ILogger _logger = logger;
    private readonly ISubscriptionService _validator = validator;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Check if the endpoint has SkipSubscriptionValidationAttribute metadata
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<SkipSubscriptionValidationAttribute>() != null)
        {
            await next(context);
            return;
        }
        // Check if the endpoint should skip validation
        var subscriptionKey = context.Request.Headers["ocp-apim-subscriptionkey"].FirstOrDefault();
        var apiPath = GetApiPath(context.Request.Path.Value);

        if(apiPath!="*" && subscriptionKey == null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(HandleInvalidResponse(StatusCodes.Status401Unauthorized));
            return;
        }

        if (apiPath != "*" && !await _validator.CheckSubscriptionValidity(subscriptionKey, apiPath))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(HandleInvalidResponse(StatusCodes.Status403Forbidden));
            return;
        }

        await next(context);
    }

    private string GetApiPath(string requestPath)
    {
        if (requestPath == "/")
        {
            return "*";
        }

        var segments = requestPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return segments.Length > 0 ? "/" + segments[0] : string.Empty;
    }

    private object HandleInvalidResponse(int status)
    {
        return status switch
        {
            StatusCodes.Status401Unauthorized => new
            {
                ErrorCode = StatusCodes.Status403Forbidden,
                Message = "Forbidden: No subscription provided"
            },
            StatusCodes.Status403Forbidden => new
            {
                ErrorCode = StatusCodes.Status403Forbidden,
                Message = $"Forbidden: Invalid subscription key provided"
            },
            _ => throw new NotImplementedException()
        };
    }
    
}

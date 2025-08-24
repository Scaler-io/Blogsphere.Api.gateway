using Serilog.Context;

namespace Blogsphere.Api.Gateway.Middlewares;

public class CorrelationHeaderEnricher : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var correlationId = GetOrGenerateCorrelationId(context);
        using(LogContext.PushProperty("ThreadId", Environment.CurrentManagedThreadId)){
            LogContext.PushProperty(LoggerConstants.CorrelationId, correlationId);
            context.Request.Headers.Append(LoggerConstants.CorrelationId, correlationId);
            await next(context);
        }
    }

    private string GetOrGenerateCorrelationId(HttpContext context)  => context?.Request.GetRequestHeaderOrdefault("CorrelationId", $"GEN-{Guid.NewGuid().ToString()}");
}

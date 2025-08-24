using System.Runtime.CompilerServices;

namespace Blogsphere.Api.Gateway.Extensions;

public static class LoggerExtensions
{
    public static ILogger Here(this ILogger logger, 
        [CallerMemberName] string member = "",
        [CallerFilePath] string caller = "")
    {
        var callerType = Path.GetFileNameWithoutExtension(caller);
        return logger.ForContext(LoggerConstants.MemberName, member)
            .ForContext(LoggerConstants.CallerType, callerType);
    }

    public static void MethodEntered(this ILogger logger)
    {
        logger.Debug(LoggerConstants.MethodEntered);
    }

    public static void MethodExited(this ILogger logger, object withResult = null)
    {
        logger.Debug(LoggerConstants.MethodExited);
        if (withResult is not null)
        {
            logger.Debug("{MethodExited} with result {@result}", LoggerConstants.MethodExited, withResult);
        }
    }

    public static ILogger WithCorrelationId(this ILogger logger, string correlationId)
    {
        return logger.ForContext(LoggerConstants.CorrelationId, correlationId);
    }

    public static ILogger WithApiProductId(this ILogger logger, object apiProductId)
    {
        return logger.ForContext(LoggerConstants.ApiProductId, apiProductId);
    }

    public static ILogger WithSubscribedApiId(this ILogger logger, object subscribedApiId)
    {
        return logger.ForContext(LoggerConstants.SubscribedApiId, subscribedApiId);
    }

    public static ILogger WithSubscriptionId(this ILogger logger, object subscriptionId)
    {
        return logger.ForContext(LoggerConstants.SubscriptionId, subscriptionId);
    }
}

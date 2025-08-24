using Destructurama;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;

namespace Blogsphere.Api.Gateway;

public static class Logging
{
    public static ILogger CreateLogger(IConfiguration configuration)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var loggingOption = configuration.GetSection(LoggingOption.OptionName).Get<LoggingOption>();
        var appConfigOption = configuration.GetSection(AppConfigOption.OptionName).Get<AppConfigOption>();
        var elasticSearchOption = configuration.GetSection(ElasticSearchOption.OptionName).Get<ElasticSearchOption>();
        
        var logIndexPattern = $"Blogsphere.Api.Gateway-{environment.ToLower().Replace(".", "-")}";

        Enum.TryParse(loggingOption.Console.LogLevel, false, out LogEventLevel minimumConsoleEventLevel);
        Enum.TryParse(loggingOption.Elastic.LogLevel, false, out LogEventLevel minimumElasticEventLevel);

        var loggerConfiguration = new LoggerConfiguration()
        .MinimumLevel.ControlledBy(new LoggingLevelSwitch(minimumConsoleEventLevel))
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
        .MinimumLevel.Override("System", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .Enrich.WithProperty(nameof(Environment.MachineName), Environment.MachineName)
        .Enrich.WithProperty(nameof(appConfigOption.ApplicationIdentifier), appConfigOption.ApplicationIdentifier)
        .Enrich.WithProperty(nameof(appConfigOption.ApplicationEnvironment), appConfigOption.ApplicationEnvironment);

        if(loggingOption.Elastic.Enabled)
        {
            loggerConfiguration.WriteTo.Elasticsearch(
                nodeUris: elasticSearchOption.Uri, 
                indexFormat: logIndexPattern, 
                restrictedToMinimumLevel: minimumConsoleEventLevel
            );
        }

        if(loggingOption.Console.Enabled)
        {
            loggerConfiguration.WriteTo.Console(
                restrictedToMinimumLevel: minimumElasticEventLevel,
                outputTemplate: loggingOption.LogOutputTemplate,
                theme: AnsiConsoleTheme.Code
            );
        }

        return loggerConfiguration
            .Destructure
            .UsingAttributes()
            .CreateLogger();
    }
}

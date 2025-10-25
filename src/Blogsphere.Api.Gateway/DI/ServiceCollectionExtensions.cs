using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Swashbuckle.AspNetCore.Filters;

namespace Blogsphere.Api.Gateway.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
    {

        // Validate configuration
        var appConfigOption = configuration
            .GetSection(AppConfigOption.OptionName)
            .Get<AppConfigOption>() ?? throw new InvalidOperationException("AppConfigOption configuration is missing");

        var identityGroupAccess = configuration
            .GetSection(IdentityGroupAccessOption.OptionName)
            .Get<IdentityGroupAccessOption>() ?? throw new InvalidOperationException("IdentityGroupAccessOption configuration is missing");

        // Add controllers and API configuration
        services.AddControllers()
            .AddNewtonsoftJson(config => {
                config.SerializerSettings.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };
                config.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
                config.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

        // Add API versioning
        services.AddEndpointsApiExplorer();
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = ApiVersion.Default;
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        var apiName = SwaggerConfiguration.ExtractApiNameFromEnvironmentVariable();
        var apiDescription = configuration["ApiDescription"];
        var apiHost = configuration["ApiOriginHost"];
        var swaggerConfiguration = new SwaggerConfiguration(apiName, apiDescription, apiHost, isDevelopment);
        services
            .AddSwaggerExamplesFromAssemblyOf<CreateProxyClusterRequestExample>()
            .AddSwaggerExamples();

        services.AddSwaggerGen(options =>
        {
            var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
            swaggerConfiguration.SetupSwaggerGenService(options, provider);
        });


        // Configure rate limiting
        services.AddRateLimiter(options => 
        {
            options.AddFixedWindowLimiter("blogsphereratelimitter", policy => 
            {
                policy.PermitLimit = 15;
                policy.Window = TimeSpan.FromSeconds(15);
                policy.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                policy.QueueLimit = 1;
            });        
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });

        // Configure OpenTelemetry
        services.AddOpenTelemetry()
            .WithTracing(tracing =>
                tracing.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(appConfigOption.ApplicationIdentifier))
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddMassTransitInstrumentation()
                .AddZipkinExporter(options => options.Endpoint = new Uri(configuration["Zipkin:Url"] ?? 
                    throw new InvalidOperationException("Zipkin:Url configuration is missing")))
                .AddJaegerExporter(options => {
                    options.AgentHost = configuration["Jaeger:AgentHost"];
                    options.AgentPort = int.Parse(configuration["Jaeger:AgentPort"]);
                })
            );

        // Configure EventBus
        services.AddMassTransit(config => 
        {
            config.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("apigateway", false));
            config.UsingRabbitMq((context, cfg) => 
            {
                var eventBus = configuration.GetSection(EventBusOption.OptionName).Get<EventBusOption>();
                cfg.Host(eventBus.Host, eventBus.VirtualHost, host => 
                {
                    host.Username(eventBus.Username);
                    host.Password(eventBus.Password);
                });
                cfg.UseMessageRetry(retry => retry.Interval(3, 1000));
                cfg.ConfigureEndpoints(context);
            });
        });


            // Configure YARP with database-driven configuration
        services.AddReverseProxy()
            .AddDatabaseConfig();

        // Register background service for config refresh
        services.AddHostedService<ProxyConfigRefreshService>();

        services.AddTransient<CorrelationHeaderEnricher>();
        services.AddTransient<RequestLoggerMiddleware>();
        services.AddTransient<GlobalExceptionMiddleware>();
        services.AddTransient<SubscriptionValidationMiddleware>();

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IPermissionMapper, PermissionMapper>();
        services.AddHttpContextAccessor();

        // Configure CORS
        services.AddCors(options => 
        {
            options.AddPolicy("blogspherecors", policy => 
                policy.WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>() ?? 
                    ["http://localhost:4200"])
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => 
            {
                options.RequireHttpsMetadata = false;
                options.Audience = identityGroupAccess.Audience;
                options.Authority = identityGroupAccess.Authority;
                options.TokenValidationParameters = new()
                {
                    ValidIssuer = identityGroupAccess.Authority,
                    ValidAudience = identityGroupAccess.Audience,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateIssuer = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy("RequiredScope", policy =>
            {
                policy.RequireClaim("scope", "apigateway:write");
                policy.RequireClaim("scope", "apigateway:read");
                policy.RequireClaim("scope", "apigateway:delete");
            });

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = HandleFrameworkValidationFailure();
        });

        services.AddValidators();
        services.AddFluentValidationAutoValidation();

        services.AddScoped<IPublishServiceFactory, PublishServiceFactory>();
        services.AddScoped(typeof(IPublishService<,>), typeof(PublishService<,>));
        
        return services;
    }

    private static Func<ActionContext, IActionResult> HandleFrameworkValidationFailure()
    {
        return context =>
        {    
            var errors = context.ModelState
            .Where(m => m.Value.Errors.Count > 0)
            .ToList();

            var validationError = new ApiValidationResponse
            {
                Errors = []
            };

            foreach (var error in errors)
            {
                foreach (var subError in error.Value.Errors)
                {
                    
                    validationError.Errors.Add(new FieldLevelError
                    {
                        Field = error.Key,
                        Message = subError.ErrorMessage,
                    });
                }
            }

            return new BadRequestObjectResult(validationError);
        };
    }
}

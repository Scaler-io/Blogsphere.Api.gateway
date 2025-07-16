using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Blogsphere.Api.Gateway.Configurations;
using Blogsphere.Api.Gateway.Infrastructure.BackgroundServices;
using Blogsphere.Api.Gateway.Infrastructure.Extensions;
using Blogsphere.Api.Gateway.Middlewares;
using Blogsphere.Api.Gateway.Models.Common;
using Blogsphere.Api.Gateway.Models.Enums;
using Blogsphere.Api.Gateway.Swagger;
using Blogsphere.Api.Gateway.Swagger.Examples;
using FluentValidation.AspNetCore;
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
                policy.PermitLimit = 4;
                policy.Window = TimeSpan.FromSeconds(12);
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
                .AddZipkinExporter(options => options.Endpoint = new Uri(configuration["Zipkin:Url"] ?? 
                    throw new InvalidOperationException("Zipkin:Url configuration is missing")))
            );

            // Configure YARP with database-driven configuration
        services.AddReverseProxy()
            .AddDatabaseConfig();

        // Register background service for config refresh
        services.AddHostedService<ProxyConfigRefreshService>();

        services.AddTransient<SubscriptionValidationMiddleware>();
        services.AddTransient<GlobalExceptionMiddleware>();

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
                    ValidateIssuer = true,
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

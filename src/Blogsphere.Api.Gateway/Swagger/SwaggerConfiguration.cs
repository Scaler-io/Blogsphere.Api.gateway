using Asp.Versioning.ApiExplorer;
using Blogsphere.Api.Gateway.Models.Constants;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Blogsphere.Api.Gateway.Swagger;

public sealed class SwaggerConfiguration
{
    private const string DefaultScheme = "http";
    private const string DefaultEnvironmentName = "";

    private readonly string _apiName;
    private readonly string _apiDescription;
    private static bool _isDevelopment;
    private static string _apiHost;

     public SwaggerConfiguration(string apiName, string apiDescription, string apiHost, bool isDevelopment)
    {
        _apiName = apiName;
        _apiDescription = apiDescription;
        _apiHost = apiHost;
        _isDevelopment = isDevelopment;
    }

    public static string ExtractApiNameFromEnvironmentVariable()
    {
        var environmentName = Environment.GetEnvironmentVariable(EnvironmentConstants.SwaggerEnvironmentName) ??
                            DefaultEnvironmentName;
        var apiName = $"Proxy API Gateway {environmentName}".Trim();
        return apiName;
    }

    public static void SetupSwaggerOptions(SwaggerOptions options)
    {
        var scheme = Environment.GetEnvironmentVariable(EnvironmentConstants.SwaggerScheme)?.ToLowerInvariant() ?? DefaultScheme;
        options.PreSerializeFilters.Add((swagger, httpReq) =>
        {
            swagger.Servers.Clear(); // clears existing servers
            swagger.Servers.Add(new OpenApiServer { Url = $"{scheme}://{_apiHost}" });
            swagger.Servers.Add(new OpenApiServer { Url = $"https://{_apiHost}" });
        });
    }

    public void SetupSwaggerGenService(SwaggerGenOptions options, IApiVersionDescriptionProvider serviceProvider)
    {
        foreach (var description in serviceProvider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = _apiName,
                Version = description.GroupName,
                Description = _apiDescription
            });
        }

        options.DocumentFilter<SwaggerBasePath>();
        // options.DocumentFilter<SwaggerRemoveVersionFromRoute>();
        options.UseInlineDefinitionsForEnums();
        options.ExampleFilters();
        options.OperationFilter<SwaggerHeaderFilter>();
        options.SchemaFilter<EnumSchemaFilter>();
        options.CustomSchemaIds(x => x.FullName);
        options.EnableAnnotations();
        if (_isDevelopment)
        {
            options.OperationFilter<SwaggerApiVersionFilter>();
        }
    }

    public static void SetupSwaggerUiOptions(SwaggerUIOptions options, IApiVersionDescriptionProvider serviceProvider)
    {
        foreach (var description in serviceProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    $"Proxy API Gateway - {description.GroupName.ToUpperInvariant()}");
        }
    }

    public static void SetupScalarUiOptions(ScalarOptions options, ApiVersionDescription description)
    {
        options.WithOpenApiRoutePattern($"/swagger/{description.GroupName}/swagger.json")
        .WithTitle($"Proxy API Gateway - {description.GroupName.ToUpperInvariant()}")
        .WithDarkModeToggle()
        .WithTheme(ScalarTheme.Saturn)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.Http)
        .WithDefaultFonts()
        .WithLayout(ScalarLayout.Modern);
    }
}

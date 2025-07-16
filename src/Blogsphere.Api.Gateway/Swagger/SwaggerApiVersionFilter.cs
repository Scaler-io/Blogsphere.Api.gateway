using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Blogsphere.Api.Gateway.Swagger;

public class SwaggerApiVersionFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= [];

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "Api-Version",
            In = ParameterLocation.Header,
            Schema = new OpenApiSchema { Type = "string" },
            Description = "Version of the api. Example v1",
            Required = true,
        });
    }
}

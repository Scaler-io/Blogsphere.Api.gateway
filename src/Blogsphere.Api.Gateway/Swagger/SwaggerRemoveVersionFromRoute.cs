using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Blogsphere.Api.Gateway.Swagger;

public class SwaggerRemoveVersionFromRoute : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var modifiedPaths = new OpenApiPaths();
        foreach(var path in swaggerDoc.Paths)
        {
            string pathWithoutVersion = path.Key[7..];
            if(string.IsNullOrEmpty(pathWithoutVersion) )
            {
                pathWithoutVersion = "/";
            }
            modifiedPaths.Add(pathWithoutVersion, path.Value);
        }

        swaggerDoc.Paths = modifiedPaths;
    }
}

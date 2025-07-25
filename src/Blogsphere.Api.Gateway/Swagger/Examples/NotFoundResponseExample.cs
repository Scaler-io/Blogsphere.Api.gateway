using Blogsphere.Api.Gateway.Models.Common;
using Blogsphere.Api.Gateway.Models.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace Blogsphere.Api.Gateway.Swagger.Examples;

public class NotFoundResponseExample : IExamplesProvider<ApiResponse>
{
    public ApiResponse GetExamples()
    {
        return new ApiResponse(ErrorCodes.NotFound, "Resource not found");
    }
}

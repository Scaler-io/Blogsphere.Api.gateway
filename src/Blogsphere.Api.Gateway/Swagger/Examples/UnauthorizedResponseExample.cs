using Swashbuckle.AspNetCore.Filters;

namespace Blogsphere.Api.Gateway.Swagger.Examples;

public class UnauthorizedResponseExample : IExamplesProvider<ApiResponse>
{
    public ApiResponse GetExamples()
    {
        return new ApiResponse(ErrorCodes.Unauthorized, "Access denied");
    }
}

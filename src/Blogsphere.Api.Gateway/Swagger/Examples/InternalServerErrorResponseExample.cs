using Blogsphere.Api.Gateway.Models.Common;
using Blogsphere.Api.Gateway.Models.Constants;
using Swashbuckle.AspNetCore.Filters;

namespace Blogsphere.Api.Gateway.Swagger.Examples;

public class InternalServerErrorResponseExample : IExamplesProvider<ApiExceptionResponse>
{
    public ApiExceptionResponse GetExamples()
    {
        return new ApiExceptionResponse(ErrorMessages.InternalServerError, "Stack Trace");
    }
}

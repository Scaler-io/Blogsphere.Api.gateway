using Blogsphere.Api.Gateway.Models.Common;
using Swashbuckle.AspNetCore.Filters;
using Blogsphere.Api.Gateway.Models.Enums;

namespace Blogsphere.Api.Gateway.Swagger.Examples;

public class ValidationResponseExample : IExamplesProvider<ApiValidationResponse>
{
    public ApiValidationResponse GetExamples()
    {
        return new ApiValidationResponse
        {
            Code = ErrorCodes.BadRequest,
            Errors = [
                new FieldLevelError {
                    Field = "Field1",
                    Message = "Field1 is required"
                },
                new FieldLevelError {
                    Field = "Field2",
                    Message = "Field2 is required"
                }
            ]
        };
    }
}

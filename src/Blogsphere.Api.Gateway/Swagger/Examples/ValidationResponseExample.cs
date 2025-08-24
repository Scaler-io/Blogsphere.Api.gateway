using Swashbuckle.AspNetCore.Filters;

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

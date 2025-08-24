namespace Blogsphere.Api.Gateway.Models.Common;

public class ApiResponse
{
    public ApiResponse(ErrorCodes code, string errorMessage = null)
    {
        Code = code;
        ErrorMessage = errorMessage ?? GetDefaultMessage(code);
    }

    public ErrorCodes Code { get; set; }
    public string ErrorMessage { get; set; }
    protected virtual string GetDefaultMessage(ErrorCodes code)
    {
        return code switch
        {
            ErrorCodes.BadRequest => ErrorMessages.BadRequest,
            ErrorCodes.NotFound => ErrorMessages.NotFound,
            ErrorCodes.Unauthorized => ErrorMessages.Unauthorized,
            ErrorCodes.OperationFailed => ErrorMessages.OperationFailed,
            ErrorCodes.InternalServerError => ErrorMessages.InternalServerError,
            ErrorCodes.NotAllowed => ErrorMessages.NotAllowed,
            _ => string.Empty
        };
    }
}

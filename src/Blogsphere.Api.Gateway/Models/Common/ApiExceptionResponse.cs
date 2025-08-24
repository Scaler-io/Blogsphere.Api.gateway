namespace Blogsphere.Api.Gateway.Models.Common;

public class ApiExceptionResponse : ApiResponse
{
    public ApiExceptionResponse(string errorMessage = "", string stackTrace = "") 
            : base(ErrorCodes.InternalServerError)
        {
            ErrorMessage = errorMessage ?? GetDefaultMessage(Code);
            StackTrace = stackTrace;
        }

        public string StackTrace { get; set; }
}

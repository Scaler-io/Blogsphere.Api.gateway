using System.Net;
using Blogsphere.Api.Gateway.Extensions;
using Blogsphere.Api.Gateway.Models.Common;
using Blogsphere.Api.Gateway.Models.Enums;
using Blogsphere.Api.Gateway.Services.Interfaces;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Blogsphere.Api.Gateway.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class BaseApiController(ILogger logger, IIdentityService identityService) : ControllerBase
{
    private ILogger Logger { get; set; } = logger;
    private readonly IIdentityService _identityService = identityService;
    protected RequestInformation RequestInformation => new()
    {
        CurrentUser = _identityService.PrepareUser(),
        CorrelationId = GetOrGenerateCorelationId()
    };
    
    protected string GetOrGenerateCorelationId() => Request?.GetRequestHeaderOrdefault("CorrelationId", $"GEN-{Guid.NewGuid().ToString()}");

    protected IActionResult OkOrFailure<T>(Result<T> result, HttpStatusCode? successCode)
    {

        if (result == null) return NotFound(new ApiResponse(ErrorCodes.NotFound));
        if (result.IsSuccess && result.Value == null) return NotFound(new ApiResponse(ErrorCodes.NotFound));
        if (result.IsSuccess && result.Value != null) return Ok(result.Value);

        return result.ErrorCode switch
        {
            ErrorCodes.BadRequest => BadRequest(new ApiValidationResponse(result.ErrorMessage)),
            ErrorCodes.InternalServerError => InternalServerError(new ApiExceptionResponse(result.ErrorMessage)),
            ErrorCodes.NotFound => NotFound(new ApiResponse(ErrorCodes.NotFound, result.ErrorMessage)),
            ErrorCodes.Unauthorized => Unauthorized(new ApiResponse(ErrorCodes.Unauthorized, result.ErrorMessage)),
            ErrorCodes.OperationFailed => BadRequest(new ApiResponse(ErrorCodes.OperationFailed, result.ErrorMessage)),
            ErrorCodes.NotAllowed => BadRequest(new ApiResponse(ErrorCodes.NotAllowed, result.ErrorMessage)),
            _ => BadRequest(new ApiResponse(ErrorCodes.BadRequest, result.ErrorMessage))
        };
    }

    private ObjectResult InternalServerError(ApiResponse response)
    {
        return new ObjectResult(response)
        {
            StatusCode = StatusCodes.Status500InternalServerError,
            ContentTypes =
            [
                "application/json"
            ]
        };
    }

    protected IActionResult ProcessValidationResult(ValidationResult validationResult)
    {
        var errors = validationResult.Errors;
        var validationError = new ApiValidationResponse()
        {
            Errors = []
        };

        validationError.Errors.AddRange(
            errors.Select(error => new FieldLevelError
            {
                Code = error.ErrorCode,
                Field = error.PropertyName,
                Message = error.ErrorMessage
            })
        );

        return new BadRequestObjectResult(validationError);
    }

    public static bool IsInvalidResult(ValidationResult validationResult)
    {
        return validationResult != null && !validationResult.IsValid;
    }
}

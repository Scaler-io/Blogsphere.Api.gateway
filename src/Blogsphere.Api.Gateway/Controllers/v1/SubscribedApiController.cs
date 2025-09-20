using Asp.Versioning;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Blogsphere.Api.Gateway.Controllers.v1;

[Authorize]
[ApiVersion("1")]
[SkipSubscriptionValidation]
public class SubscribedApiController(ILogger logger, IIdentityService identityService, ISubscribedApiManageService subscribedApiManageService) 
    : BaseApiController(logger, identityService)
{
    private readonly ISubscribedApiManageService _subscribedApiManageService = subscribedApiManageService;

    [HttpGet]
    [SwaggerHeader("CorrelationId", Description ="The correlation id of the request")]
    [SwaggerOperation("GetAllSubscribedApis", Summary = "Get all subscribed apis")]
    // 200
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SubscribedApiPaginatedResponseExample))]
    [ProducesResponseType(typeof(PaginatedResult<SubscribedApiDto>), StatusCodes.Status200OK)]
    // 401
    [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedResponseExample))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    // 500
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExample))]
    [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
    [RequirePermission(ApiAccess.CanViewSystemSettings, ApiScopes.GatewayRead)]
    public async Task<IActionResult> GetAllSubscribedApis([FromQuery] PaginationRequest request)
    {
        Logger.Here().MethodEntered();
        var result = await _subscribedApiManageService.GetAllSubscribedApisAsync(request);
        Logger.Here().MethodExited();
        return OkOrFailure(result, HttpStatusCode.OK);
    }

    [HttpGet("product/{productId}")]
    [SwaggerHeader("CorrelationId", Description ="The correlation id of the request")]
    [SwaggerOperation("GetAllSubscribedApisByProductId", Summary = "Get all subscribed apis by product id")]
    // 200
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SubscribedApiListResponseExample))]
    [ProducesResponseType(typeof(List<SubscribedApiDto>), StatusCodes.Status200OK)]
    // 401
    [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedResponseExample))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [RequirePermission(ApiAccess.CanViewSystemSettings, ApiScopes.GatewayRead)]
    public async Task<IActionResult> GetAllSubscribedApisByProductId([FromRoute] Guid productId)
    {
        Logger.Here().MethodEntered();
        var result = await _subscribedApiManageService.GetSubscribedApisByProductIdAsync(productId);
        Logger.Here().MethodExited();
        return OkOrFailure(result, HttpStatusCode.OK);
    }

    [HttpGet("{id}")]
    [SwaggerHeader("CorrelationId", Description ="The correlation id of the request")]
    [SwaggerOperation("GetSubscribedApiById", Summary = "Get a subscribed api by id")]
    // 200
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SubscribedApiResponseExample))]
    [ProducesResponseType(typeof(SubscribedApiDto), StatusCodes.Status200OK)]
    // 401
    [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedResponseExample))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    // 404
    [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundResponseExample))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    // 500
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExample))]
    [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
    [RequirePermission(ApiAccess.CanViewSystemSettings, ApiScopes.GatewayRead)]
    public async Task<IActionResult> GetSubscribedApiById([FromRoute] Guid id)
    {
        Logger.Here().MethodEntered();
        var result = await _subscribedApiManageService.GetSubscribedApiByIdAsync(id);
        Logger.Here().MethodExited();
        return OkOrFailure(result, HttpStatusCode.OK);
    }


    [HttpPost]
    [SwaggerHeader("CorrelationId", Description ="The correlation id of the request")]
    [SwaggerOperation("CreateSubscribedApi", Summary = "Create a new subscribed api")]
    [SwaggerRequestExample(typeof(CreateSubscribedApiRequest), typeof(CreateSubscribedApiRequestExample))]
    // 200
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SubscribedApiResponseExample))]
    [ProducesResponseType(typeof(SubscribedApiDto), StatusCodes.Status200OK)]
    // 400
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ValidationResponseExample))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    // 401
    [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedResponseExample))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    // 500
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExample))]
    [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
    [RequirePermission(ApiAccess.CanUpdateSystemSettings, ApiScopes.GatewayWrite)]
    public async Task<IActionResult> CreateSubscribedApi([FromBody] CreateSubscribedApiRequest request)
    {
        Logger.Here().MethodEntered();
        var result = await _subscribedApiManageService.CreateSubscribedApiAsync(request, RequestInformation);
        Logger.Here().MethodExited();
        return OkOrFailure(result, HttpStatusCode.OK);
    }

    [HttpPut]
    [SwaggerHeader("CorrelationId", Description ="The correlation id of the request")]
    [SwaggerOperation("UpdateSubscribedApi", Summary = "Update a subscribed api")]
    [SwaggerRequestExample(typeof(UpdateSubscribedApiRequest), typeof(UpdateSubscribedApiRequestExample))]
    // 200
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SubscribedApiResponseExample))]
    [ProducesResponseType(typeof(SubscribedApiDto), StatusCodes.Status200OK)]  
    // 400
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ValidationResponseExample))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    // 401
    [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedResponseExample))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]  
    // 404
    [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundResponseExample))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    // 500
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExample))]
    [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
    [RequirePermission(ApiAccess.CanUpdateSystemSettings, ApiScopes.GatewayWrite)]
    public async Task<IActionResult> UpdateSubscribedApi([FromBody] UpdateSubscribedApiRequest request)
    {
        Logger.Here().MethodEntered();
        var result = await _subscribedApiManageService.UpdateSubscribedApiAsync(request, RequestInformation);
        Logger.Here().MethodExited();
        return OkOrFailure(result, HttpStatusCode.OK);
    }

    [HttpDelete("{id}")]
    [SwaggerHeader("CorrelationId", Description ="The correlation id of the request")]
    [SwaggerOperation("DeleteSubscribedApi", Summary = "Delete a subscribed api")]
    // 200
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(bool))]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    // 401
    [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedResponseExample))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    // 404
    [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundResponseExample))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    // 500
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExample))]
    [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
    [RequirePermission(ApiAccess.CanUpdateSystemSettings, ApiScopes.GatewayDelete)]
    public async Task<IActionResult> DeleteSubscribedApi([FromRoute] Guid id)
    {
        Logger.Here().MethodEntered();
        var result = await _subscribedApiManageService.DeleteSubscribedApiAsync(id);
        Logger.Here().MethodExited();
        return OkOrFailure(result, HttpStatusCode.OK);
    }
}

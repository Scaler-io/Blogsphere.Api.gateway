using System.Net;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Blogsphere.Api.Gateway.Controllers.v1;

[Authorize]
[ApiVersion("1.0")]
[SkipSubscriptionValidation]
public class SubscriptionController(ILogger logger, IIdentityService identityService, ISubscriptionManageService subscriptionManageService) 
    : BaseApiController(logger, identityService)    
{
    private readonly ISubscriptionManageService _subscriptionManageService = subscriptionManageService;

    [HttpGet]
    [SwaggerHeader("CorrelationId", Description ="The correlation id of the request")]
    [SwaggerOperation("GetAllSubscriptions", Summary = "Get all subscriptions")]
    // 200
    [ProducesResponseType(typeof(PaginatedResult<SubscriptionDto>), StatusCodes.Status200OK)]
    // 401
    [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedResponseExample))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    // 500
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExample))]
    [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
    [RequirePermission(ApiAccess.CanViewSystemSettings, ApiScopes.GatewayRead)]
    public async Task<IActionResult> GetAllSubscriptions([FromQuery] PaginationRequest request)
    {
        Logger.Here().MethodEntered();
        var result = await _subscriptionManageService.GetAllSubscriptionsAsync(request);
        Logger.Here().MethodExited();
        return OkOrFailure(result, HttpStatusCode.OK);
    }

    [HttpGet("{id}")]
    [SwaggerHeader("CorrelationId", Description ="The correlation id of the request")]
    [SwaggerOperation("GetSubscriptionById", Summary = "Get a subscription by id")]
    // 200
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SubscriptionResponseExample))]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status200OK)]
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
    public async Task<IActionResult> GetSubscriptionById([FromRoute] Guid id)
    {
        Logger.Here().MethodEntered();
        var result = await _subscriptionManageService.GetSubscriptionByIdAsync(id);
        Logger.Here().MethodExited();
        return OkOrFailure(result, HttpStatusCode.OK);
    }

    [HttpPost]
    [SwaggerHeader("CorrelationId", Description ="The correlation id of the request")]
    [SwaggerOperation("CreateSubscription", Summary = "Create a new subscription")]
    [SwaggerRequestExample(typeof(CreateSubscriptionRequest), typeof(CreateSubscriptionRequestExample))]

    // 200
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SubscriptionResponseExample))]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status200OK)]
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
    public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionRequest request)

    {
        Logger.Here().MethodEntered();
        var result = await _subscriptionManageService.CreateSubscriptionAsync(request, RequestInformation);
        Logger.Here().MethodExited();
        return OkOrFailure(result, HttpStatusCode.OK);
    }

    [HttpDelete("{id}")]
    [SwaggerHeader("CorrelationId", Description ="The correlation id of the request")]
    [SwaggerOperation("DeleteSubscription", Summary = "Delete a subscription")]
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
    public async Task<IActionResult> DeleteSubscription([FromRoute] Guid id)
    {
        Logger.Here().MethodEntered();
        var result = await _subscriptionManageService.DeleteSubscriptionAsync(id);
        Logger.Here().MethodExited();
        return OkOrFailure(result, HttpStatusCode.OK);
    }
}

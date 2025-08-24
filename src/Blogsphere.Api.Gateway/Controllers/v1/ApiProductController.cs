using System.Net;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Blogsphere.Api.Gateway.Controllers.v1;

[Authorize]
[ApiVersion("1")]
[SkipSubscriptionValidation]
public class ApiProductController(
    ILogger logger,
    IIdentityService identityService,
    IApiProductManageService apiProductManageService) 
: BaseApiController(logger, identityService)
{
    private readonly IApiProductManageService _apiProductManageService = apiProductManageService;

    [HttpGet]
    [SwaggerHeader("CorrelationId", Description ="The correlation id of the request")]
    [SwaggerOperation("GetAllProductApis", Summary = "Get all api products")]
    // 200
    [ProducesResponseType(typeof(PaginatedResult<ApiProductDto>), StatusCodes.Status200OK)]
    // 401
    [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedResponseExample))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    // 500
    [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExample))]
    [RequirePermission(ApiAccess.CanViewSystemSettings, ApiScopes.GatewayRead)]
    public async Task<IActionResult> GetAllProductApis([FromQuery] PaginationRequest request)
    {
        Logger.Here().MethodEntered();
        var result = await _apiProductManageService.GetAllApiProductsAsync(request);
        Logger.Here().MethodExited();
        return OkOrFailure(result, HttpStatusCode.OK);
    }

    [HttpGet("{id}")]
    [SwaggerHeader("CorrelationId", Description ="The correlation id of the request")]
    [SwaggerOperation("GetApiProductById", Summary = "Get an api product by id")]
    // 200
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ApiProductResponseExample))]
    [ProducesResponseType(typeof(ApiProductDto), StatusCodes.Status200OK)]
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
    public async Task<IActionResult> GetApiProductById([FromRoute] Guid id)
    {
        Logger.Here().MethodEntered();
        var result = await _apiProductManageService.GetApiProductByIdAsync(id);
        Logger.Here().MethodExited();
        return OkOrFailure(result, HttpStatusCode.OK);
    }

    [HttpPost]
    [SwaggerHeader("CorrelationId", Description ="The correlation id of the request")]
    [SwaggerOperation("CreateApiProduct", Summary = "Create a new api product")]
    [SwaggerRequestExample(typeof(CreateApiProductRequest), typeof(CreateApiProductRequestExample))]
    // 200 
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ApiProductResponseExample))]
    [ProducesResponseType(typeof(ApiProductDto), StatusCodes.Status200OK)]
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
    public async Task<IActionResult> CreateApiProduct([FromBody] CreateApiProductRequest request)
    {
        Logger.Here().MethodEntered();
        var result = await _apiProductManageService.CreateApiProductAsync(request, RequestInformation);
        Logger.Here().MethodExited();
        return OkOrFailure(result, HttpStatusCode.OK);
    }
    
    [HttpDelete("{id}")]
    [SwaggerHeader("CorrelationId", Description ="The correlation id of the request")]
    [SwaggerOperation("DeleteApiProduct", Summary = "Delete an api product")]
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
    public async Task<IActionResult> DeleteApiProduct([FromRoute] Guid id)
    {
        Logger.Here().MethodEntered();
        var result = await _apiProductManageService.DeleteApiProductAsync(id, RequestInformation);
        Logger.Here().MethodExited();
        return OkOrFailure(result, HttpStatusCode.OK);
    }
}

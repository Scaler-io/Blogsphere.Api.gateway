using System.Net;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Authorization;

namespace Blogsphere.Api.Gateway.Controllers.v1;

[Authorize]
[ApiVersion("1")]
[SkipSubscriptionValidation]
public class ProxyRouteController(
    IProxyRouteService routeService,
    DatabaseProxyConfigProvider configProvider,
    ILogger logger,
    IIdentityService identityService) : BaseApiController(logger, identityService)
{
    private readonly IProxyRouteService _routeService = routeService;
    private readonly DatabaseProxyConfigProvider _configProvider = configProvider;
    private readonly ILogger _logger = logger;

    [HttpGet]
    [SwaggerHeader("CorrelationId", Description = "Unique identifier for tracing the request through the system")]
    [SwaggerOperation(OperationId = "GetRoutes", Summary = "Retrieves all proxy routes", Description = "Returns a paginated list of all proxy routes in the system")]
    // 200
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(PaginatedProxyRouteResponseExample))]
    [ProducesResponseType(typeof(PaginatedResult<ProxyRouteSearchableDto>), StatusCodes.Status200OK)]
    // 400
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ValidationResponseExample))]
    [ProducesResponseType(typeof(ApiValidationResponse), StatusCodes.Status400BadRequest)]
    // 500
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExample))]
    [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
    [RequirePermission(ApiAccess.CanViewSystemSettings, "apigateway:read")]
    public async Task<IActionResult> GetRoutes(
        [FromQuery, SwaggerParameter("The page number")] int pageNumber = 1,
        [FromQuery, SwaggerParameter("The page size")] int pageSize = 10)
    {
        _logger.Here().MethodEntered();
        var result = await _routeService.GetAllAsync(new PaginationRequest { PageNumber = pageNumber, PageSize = pageSize });
        _logger.Here().MethodExited();
        return OkOrFailure(result, HttpStatusCode.OK);
    }

    [HttpGet("{id:guid}")]
    [SwaggerHeader("CorrelationId", Description = "Unique identifier for tracing the request through the system")]
    [SwaggerOperation(OperationId = "GetRoute", Summary = "Retrieves a specific proxy route", Description = "Returns detailed information about a specific proxy route")]
    // 200
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ProxyRouteResponseExample))]
    [ProducesResponseType(typeof(ProxyRouteDto), StatusCodes.Status200OK)]
    // 404
    [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundResponseExample))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    // 500
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExample))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
    [RequirePermission(ApiAccess.CanViewSystemSettings)]
    public async Task<IActionResult> GetRoute(
        [FromRoute, SwaggerParameter("The unique identifier of the route")] Guid id)
    {
        _logger.Here().MethodEntered();
        var result = await _routeService.GetByIdAsync(id);
        _logger.Here().MethodExited();
        return OkOrFailure(result, HttpStatusCode.OK);
    }

    [HttpPost]
    [SwaggerHeader("CorrelationId", Description = "Unique identifier for tracing the request through the system")]
    [SwaggerOperation(OperationId = "CreateRoute", Summary = "Creates a new proxy route", Description = "Creates a new proxy route with the specified configuration")]
    [SwaggerRequestExample(typeof(CreateProxyRouteRequest), typeof(CreateProxyRouteRequestExample))]
    // 201
    [SwaggerResponseExample(StatusCodes.Status201Created, typeof(ProxyRouteResponseExample))]
    [ProducesResponseType(typeof(ProxyRouteDto), StatusCodes.Status201Created)]
    // 400
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ValidationResponseExample))]
    [ProducesResponseType(typeof(ApiValidationResponse), StatusCodes.Status400BadRequest)]
    // 500
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExample))]
    [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
    [RequirePermission(ApiAccess.CanUpdateSystemSettings)]
    public async Task<IActionResult> CreateRoute(
        [FromBody, SwaggerRequestBody("The route creation request")] CreateProxyRouteRequest request)
    {
        _logger.Here().MethodEntered();
        
        var result = await _routeService.CreateFromRequestAsync(request, RequestInformation);
        
        if (result.IsSuccess)
        {
            _configProvider.Update(); // Refresh YARP configuration
        }

        _logger.Here().MethodExited();
        return OkOrFailure(result, HttpStatusCode.Created);
    }

    [HttpPut("{id:guid}")]
    [SwaggerHeader("CorrelationId", Description = "Unique identifier for tracing the request through the system")]
    [SwaggerOperation(OperationId = "UpdateRoute", Summary = "Updates an existing proxy route", Description = "Updates the configuration of an existing proxy route")]
    [SwaggerRequestExample(typeof(UpdateProxyRouteRequest), typeof(CreateProxyRouteRequestExample))]
    // 200
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ProxyRouteResponseExample))]
    [ProducesResponseType(typeof(ProxyRouteDto), StatusCodes.Status200OK)]
    // 400
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ValidationResponseExample))]
    [ProducesResponseType(typeof(ApiValidationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    // 500
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExample))]
    [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
    [RequirePermission(ApiAccess.CanUpdateSystemSettings)]
    public async Task<IActionResult> UpdateRoute(
        [FromRoute, SwaggerParameter("The unique identifier of the route to update")] Guid id,
        [FromBody, SwaggerRequestBody("The route update request")] UpdateProxyRouteRequest request)
    {
        _logger.Here().MethodEntered();
        
        var result = await _routeService.UpdateFromRequestAsync(id, request, RequestInformation);
        
        if (result.IsSuccess)
        {
            _configProvider.Update(); // Refresh YARP configuration
        }

        _logger.Here().MethodExited();
        return OkOrFailure(result, HttpStatusCode.OK);
    }

    [HttpDelete("{id:guid}")]
    [SwaggerHeader("CorrelationId", Description = "Unique identifier for tracing the request through the system")]
    [SwaggerOperation(OperationId = "DeleteRoute", Summary = "Deletes a proxy route", Description = "Removes a proxy route from the system")]
    // 204
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    // 404
    [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundResponseExample))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    // 500
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExample))]
    [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
    [RequirePermission(ApiAccess.CanUpdateSystemSettings)]
    public async Task<IActionResult> DeleteRoute(
        [FromRoute, SwaggerParameter("The unique identifier of the route to delete")] Guid id)
    {
        _logger.Here().MethodEntered();
        var result = await _routeService.DeleteAsync(id, RequestInformation);
        
        if (result.IsSuccess)
        {
            _configProvider.Update(); // Refresh YARP configuration
        }

        _logger.Here().MethodExited();
        return OkOrFailure(result, HttpStatusCode.NoContent);
    }
} 
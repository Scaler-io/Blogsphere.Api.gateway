using System.Net;
using Asp.Versioning;
using AutoMapper;
using Blogsphere.Api.Gateway.Extensions;
using Blogsphere.Api.Gateway.Infrastructure.Yarp;
using Blogsphere.Api.Gateway.Models.Common;
using Blogsphere.Api.Gateway.Models.DTOs;
using Blogsphere.Api.Gateway.Models.Requests;
using Blogsphere.Api.Gateway.Services.Interfaces;
using Blogsphere.Api.Gateway.Swagger.Examples;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.ComponentModel.DataAnnotations;
using Blogsphere.Api.Gateway.Swagger;
using Blogsphere.Api.Gateway.Filters;
using Blogsphere.Api.Gateway.Models.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Blogsphere.Api.Gateway.Controllers.v1;

[Authorize]
[ApiVersion("1")]
[SkipSubscriptionValidation]
public class ProxyRouteController(
    IProxyRouteService routeService,
    DatabaseProxyConfigProvider configProvider,
    ILogger logger,
    IIdentityService identityService,
    IMapper mapper) : BaseApiController(logger, identityService)
{
    private readonly IProxyRouteService _routeService = routeService;
    private readonly DatabaseProxyConfigProvider _configProvider = configProvider;
    private readonly ILogger _logger = logger;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    [SwaggerHeader("CorrelationId", Description = "Unique identifier for tracing the request through the system")]
    [SwaggerOperation(OperationId = "GetRoutes", Summary = "Retrieves all proxy routes", Description = "Returns a paginated list of all proxy routes in the system")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(PaginatedProxyRouteResponseExample))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ValidationResponseExample))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExample))]
    [ProducesResponseType(typeof(PaginatedResult<ProxyRouteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiValidationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
    [RequirePermission(ApiAccess.CanViewSystemSettings)]
    public async Task<IActionResult> GetRoutes(
        [FromQuery, Range(1, int.MaxValue), SwaggerParameter("Page number for pagination")] int pageNumber = 1,
        [FromQuery, Range(1, 100), SwaggerParameter("Number of items per page")] int pageSize = 10)
    {
        _logger.Here().MethodEntered();
        var result = await _routeService.GetAllAsync(
            new PaginationRequest { PageNumber = pageNumber, PageSize = pageSize });
        _logger.Here().MethodExited();
        return OkOrFailure(result, HttpStatusCode.OK);
    }

    [HttpGet("{id:guid}")]
    [SwaggerHeader("CorrelationId", Description = "Unique identifier for tracing the request through the system")]
    [SwaggerOperation(OperationId = "GetRoute", Summary = "Retrieves a specific proxy route", Description = "Returns detailed information about a specific proxy route")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ProxyRouteResponseExample))]
    [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundResponseExample))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExample))]
    [ProducesResponseType(typeof(ProxyRouteDto), StatusCodes.Status200OK)]
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
    [SwaggerResponseExample(StatusCodes.Status201Created, typeof(ProxyRouteResponseExample))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ValidationResponseExample))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExample))]
    [ProducesResponseType(typeof(ProxyRouteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiValidationResponse), StatusCodes.Status400BadRequest)]
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
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ProxyRouteResponseExample))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ValidationResponseExample))]
    [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundResponseExample))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExample))]
    [ProducesResponseType(typeof(ProxyRouteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiValidationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
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
    [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundResponseExample))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExample))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
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
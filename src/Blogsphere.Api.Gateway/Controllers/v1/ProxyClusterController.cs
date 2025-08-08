using System.Net;
using Asp.Versioning;
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
using Blogsphere.Api.Gateway.Swagger;
using Microsoft.AspNetCore.Authorization;
using Blogsphere.Api.Gateway.Filters;
using Blogsphere.Api.Gateway.Models.Enums;
using Blogsphere.Api.Gateway.Models.DTOs.Search;

namespace Blogsphere.Api.Gateway.Controllers.v1;

[Authorize]
[ApiVersion("1")]
[SkipSubscriptionValidation]
public class ProxyClusterController(
    IProxyClusterService clusterService,
    DatabaseProxyConfigProvider configProvider,
    ILogger logger,
    IIdentityService identityService) : BaseApiController(logger, identityService)
{
    private readonly IProxyClusterService _clusterService = clusterService;
    private readonly DatabaseProxyConfigProvider _configProvider = configProvider;
    private readonly ILogger _logger = logger;

    [HttpGet]
    [SwaggerHeader("CorrelationId", Description = "Unique identifier for tracing the request through the system")]
    [SwaggerOperation(OperationId = "GetClusters", Summary = "Retrieves all proxy clusters", Description = "Returns a paginated list of all proxy clusters in the system")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(PaginatedProxyClusterResponseExample))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ValidationResponseExample))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExample))]
    [ProducesResponseType(typeof(PaginatedResult<ProxyClusterSearchableDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiValidationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
    [RequirePermission(ApiAccess.CanViewSystemSettings)]
    public async Task<IActionResult> GetClusters(
        [FromQuery, SwaggerParameter("The page number")] int pageNumber = 1,
        [FromQuery, SwaggerParameter("The page size")] int pageSize = 10)
    {
        _logger.Here().MethodEntered();
        var result = await _clusterService.GetAllAsync(new PaginationRequest { PageNumber = pageNumber, PageSize = pageSize });
        _logger.Here().MethodExited();
        return OkOrFailure(result, HttpStatusCode.OK);
    }

    [HttpGet("{id:guid}")]
    [SwaggerHeader("CorrelationId", Description = "Unique identifier for tracing the request through the system")]
    [SwaggerOperation(OperationId = "GetCluster", Summary = "Retrieves a specific proxy cluster", Description = "Returns detailed information about a specific proxy cluster")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ProxyClusterResponseExample))]
    [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundResponseExample))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExample))]
    [ProducesResponseType(typeof(ProxyClusterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
    [RequirePermission(ApiAccess.CanViewSystemSettings)]
    public async Task<IActionResult> GetCluster(
        [FromRoute, SwaggerParameter("The unique identifier of the cluster")] Guid id)
    {
        _logger.Here().MethodEntered();
        var result = await _clusterService.GetByIdAsync(id);
        _logger.Here().MethodExited();
        return OkOrFailure(result, HttpStatusCode.OK);
    }

    [HttpPost]
    [SwaggerHeader("CorrelationId", Description = "Unique identifier for tracing the request through the system")]
    [SwaggerOperation(OperationId = "CreateCluster", Summary = "Creates a new proxy cluster", Description = "Creates a new proxy cluster with the specified configuration")]
    [SwaggerRequestExample(typeof(CreateProxyClusterRequest), typeof(CreateProxyClusterRequestExample))]
    [SwaggerResponseExample(StatusCodes.Status201Created, typeof(ProxyClusterResponseExample))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ValidationResponseExample))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExample))]
    [ProducesResponseType(typeof(ProxyClusterDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiValidationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
    [RequirePermission(ApiAccess.CanUpdateSystemSettings)]
    public async Task<IActionResult> CreateCluster(
        [FromBody, SwaggerRequestBody("The cluster creation request")] CreateProxyClusterRequest request)
    {
        _logger.Here().MethodEntered();
        
        var result = await _clusterService.CreateFromRequestAsync(request, RequestInformation);
        
        if (result.IsSuccess)
        {
            _configProvider.Update(); // Refresh YARP configuration
        }

        _logger.Here().MethodExited();
        return OkOrFailure(result, HttpStatusCode.Created);
    }

    [HttpPut("{id:guid}")]
    [SwaggerHeader("CorrelationId", Description = "Unique identifier for tracing the request through the system")]
    [SwaggerOperation(OperationId = "UpdateCluster", Summary = "Updates an existing proxy cluster", Description = "Updates the configuration of an existing proxy cluster")]
    [SwaggerRequestExample(typeof(UpdateProxyClusterRequest), typeof(CreateProxyClusterRequestExample))]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ProxyClusterResponseExample))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ValidationResponseExample))]
    [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundResponseExample))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExample))]
    [ProducesResponseType(typeof(ProxyClusterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiValidationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
    [RequirePermission(ApiAccess.CanUpdateSystemSettings)]
    public async Task<IActionResult> UpdateCluster(
        [FromRoute, SwaggerParameter("The unique identifier of the cluster to update")] Guid id,
        [FromBody, SwaggerRequestBody("The cluster update request")] UpdateProxyClusterRequest request)
    {
        _logger.Here().MethodEntered();
        
        var result = await _clusterService.UpdateFromRequestAsync(id, request, RequestInformation);
        
        if (result.IsSuccess)
        {
            _configProvider.Update(); // Refresh YARP configuration
        }

        _logger.Here().MethodExited();
        return OkOrFailure(result, HttpStatusCode.OK);
    }

    [HttpDelete("{id:guid}")]
    [SwaggerHeader("CorrelationId", Description = "Unique identifier for tracing the request through the system")]
    [SwaggerOperation(OperationId = "DeleteCluster", Summary = "Deletes a proxy cluster", Description = "Removes a proxy cluster from the system")]
    [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundResponseExample))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExample))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
    [RequirePermission(ApiAccess.CanUpdateSystemSettings)]
    public async Task<IActionResult> DeleteCluster(
        [FromRoute, SwaggerParameter("The unique identifier of the cluster to delete")] Guid id)
    {
        _logger.Here().MethodEntered();
        var result = await _clusterService.DeleteAsync(id, RequestInformation);
        
        if (result.IsSuccess)
        {
            _configProvider.Update(); // Refresh YARP configuration
        }

        _logger.Here().MethodExited();
        return OkOrFailure(result, HttpStatusCode.NoContent);
    }
} 
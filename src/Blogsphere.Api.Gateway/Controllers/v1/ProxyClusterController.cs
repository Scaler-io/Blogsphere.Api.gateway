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
    // 200
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(PaginatedProxyClusterResponseExample))]
    [ProducesResponseType(typeof(PaginatedResult<ProxyClusterSearchableDto>), StatusCodes.Status200OK)]
    // 400
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ValidationResponseExample))]
    [ProducesResponseType(typeof(ApiValidationResponse), StatusCodes.Status400BadRequest)]
    // 500
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExample))]
    [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
    [RequirePermission(ApiAccess.CanViewSystemSettings, "apigateway:read")]
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
    // 200
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ProxyClusterResponseExample))]
    [ProducesResponseType(typeof(ProxyClusterDto), StatusCodes.Status200OK)]
    // 404
    [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundResponseExample))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    // 500
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExample))]
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
    // 201
    [SwaggerResponseExample(StatusCodes.Status201Created, typeof(ProxyClusterResponseExample))]
    [ProducesResponseType(typeof(ProxyClusterDto), StatusCodes.Status201Created)]
    // 400
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ValidationResponseExample))]
    [ProducesResponseType(typeof(ApiValidationResponse), StatusCodes.Status400BadRequest)]
    // 500
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExample))]
    [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
    [RequirePermission(ApiAccess.CanUpdateSystemSettings, "apigateway:write")]
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
    // 200
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ProxyClusterResponseExample))]
    [ProducesResponseType(typeof(ProxyClusterDto), StatusCodes.Status200OK)]
    // 400
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ValidationResponseExample))]
    [ProducesResponseType(typeof(ApiValidationResponse), StatusCodes.Status400BadRequest)]
    // 404
    [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundResponseExample))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    // 500
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExample))]
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
    // 204
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    // 404
    [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundResponseExample))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    // 500
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExample))]
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
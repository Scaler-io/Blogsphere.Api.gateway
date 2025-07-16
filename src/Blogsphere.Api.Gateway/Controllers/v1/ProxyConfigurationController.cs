using Asp.Versioning;
using Blogsphere.Api.Gateway.Extensions;
using Blogsphere.Api.Gateway.Infrastructure.Yarp;
using Blogsphere.Api.Gateway.Models.Common;
using Blogsphere.Api.Gateway.Swagger;
using Blogsphere.Api.Gateway.Swagger.Examples;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Blogsphere.Api.Gateway.Controllers.v1;

[ApiVersion("1")]
[SkipSubscriptionValidation]
public class ProxyConfigurationController(
    DatabaseProxyConfigProvider configProvider,
    ILogger logger) : BaseApiController(logger)
{
    private readonly DatabaseProxyConfigProvider _configProvider = configProvider;
    private readonly ILogger _logger = logger;

    [HttpPost("refresh")]
    [SwaggerHeader("CorrelationId", Description = "Unique identifier for tracing the request through the system")]
    [SwaggerOperation(OperationId = "RefreshConfiguration", Summary = "Refreshes the YARP configuration", Description = "Forces an immediate refresh of the YARP reverse proxy configuration")]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExample))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
    public IActionResult RefreshConfiguration()
    {
        _logger.Here().MethodEntered();
        _configProvider.Update();
        _logger.Here().MethodExited();
        return Ok();
    }
} 
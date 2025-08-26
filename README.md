# Blogsphere API Gateway

A dynamic, configurable API Gateway built with .NET 8 and YARP (Yet Another Reverse Proxy) for the Blogsphere microservices architecture.

## Features

- 🚀 Dynamic routing configuration via REST API
- 🔒 Built-in security features and headers
- 📊 Rate limiting and request throttling
- 🔄 Real-time configuration updates
- 📝 Comprehensive logging with Serilog and Elasticsearch
- 📈 OpenTelemetry integration with Zipkin
- 🗄️ PostgreSQL for configuration storage
- 🔍 Swagger/OpenAPI documentation with Scalar UI
- 🎯 Health check endpoints
- 🌐 CORS support
- 📦 API product and subscription management
- 🔐 Role-based access control with JWT authentication
- 📡 Event-driven architecture with MassTransit and RabbitMQ
- 🏗️ Clean architecture with repository pattern
- 📋 Comprehensive API documentation with examples

## Prerequisites

- .NET 8.0 SDK
- PostgreSQL 13+
- RabbitMQ (for event bus)
- Elasticsearch (for logging - optional)
- Zipkin (for distributed tracing - optional)
- Docker (optional)
- Git

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/your-org/blogsphere-api-gateway.git
cd blogsphere-api-gateway
```

### 2. Configure Application Settings

Update the configuration in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BlogsphereProxy;Username=admin;Password=admin"
  },
  "AppConfigurations": {
    "ApplicationIdentifier": "Blogsphere.Api.Gateway",
    "ApplicationEnvironment": "Development"
  },
  "EventBus": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest",
    "VirtualHost": "/",
    "Port": 5672
  },
  "Elasticsearch": {
    "Uri": "http://localhost:9200"
  },
  "Zipkin": {
    "Url": "http://localhost:9411/api/v2/spans"
  },
  "IdentityGroupAccess": {
    "Authority": "http://localhost:5000",
    "Audience": "blogsphere.api.gateway"
  }
}
```

### 3. Run Migrations

```bash
dotnet ef database update
```

### 4. Build and Run

```bash
dotnet build
dotnet run
```

The API Gateway will be available at:

- API: http://localhost:8000
- Swagger UI: http://localhost:8000/swagger
- Scalar API Documentation: http://localhost:8000/scalar/v1

## Configuration

### Application Settings

Key configuration options in `appsettings.json`:

```json
{
  "AppConfig": {
    "ApplicationIdentifier": "blogsphere-gateway"
  },
  "AllowedOrigins": ["http://localhost:4200"],
  "Zipkin": {
    "Url": "http://localhost:9411"
  }
}
```

### Rate Limiting

Default configuration:

- 4 requests per 12 seconds
- Queue limit: 1
- Processing order: Oldest first

Customize in `Program.cs`:

```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("blogsphereratelimitter", policy =>
    {
        policy.PermitLimit = 4;
        policy.Window = TimeSpan.FromSeconds(12);
        policy.QueueLimit = 1;
    });
});
```

## API Documentation

### Base URL

```
http://localhost:8000/api/v1
```

### Key Endpoints

#### Proxy Management

**Clusters Management**

```http
GET    /api/v1/proxycluster            # List all proxy clusters
GET    /api/v1/proxycluster/{id}       # Get cluster by ID
POST   /api/v1/proxycluster           # Create cluster
PUT    /api/v1/proxycluster/{id}      # Update cluster
DELETE /api/v1/proxycluster/{id}      # Delete cluster
```

**Routes Management**

```http
GET    /api/v1/proxyroute            # List all proxy routes
GET    /api/v1/proxyroute/{id}       # Get route by ID
POST   /api/v1/proxyroute           # Create route
PUT    /api/v1/proxyroute/{id}      # Update route
DELETE /api/v1/proxyroute/{id}      # Delete route
```

**Configuration Management**

```http
POST   /api/v1/proxyconfiguration/refresh     # Force configuration refresh
```

#### Subscription Management

**API Products**

```http
GET    /api/v1/apiproduct            # List all API products
GET    /api/v1/apiproduct/{id}       # Get API product by ID
POST   /api/v1/apiproduct           # Create API product
DELETE /api/v1/apiproduct/{id}      # Delete API product
```

**Subscriptions**

```http
GET    /api/v1/subscription            # List all subscriptions
GET    /api/v1/subscription/{id}       # Get subscription by ID
POST   /api/v1/subscription           # Create subscription
DELETE /api/v1/subscription/{id}      # Delete subscription
```

**Subscribed APIs**

```http
GET    /api/v1/subscribedapi            # List all subscribed APIs
GET    /api/v1/subscribedapi/{id}       # Get subscribed API by ID
POST   /api/v1/subscribedapi           # Create subscribed API
PUT    /api/v1/subscribedapi/{id}      # Update subscribed API
DELETE /api/v1/subscribedapi/{id}      # Delete subscribed API
```

#### Required Headers

All endpoints require:

- `CorrelationId`: Unique identifier for request tracing
- `Content-Type`: application/json
- `Authorization`: Bearer JWT token (for authenticated endpoints)

### Example: Creating a Route

```bash
# 1. Create a cluster
curl -X POST http://localhost:8000/api/v1/proxycluster \
  -H "Content-Type: application/json" \
  -H "CorrelationId: your-correlation-id" \
  -H "Authorization: Bearer your-jwt-token" \
  -d '{
    "clusterId": "auth-service",
    "destinations": [
      {
        "destinationId": "auth-primary",
        "address": "http://auth-service:8080"
      }
    ]
  }'

# 2. Create a route
curl -X POST http://localhost:8000/api/v1/proxyroute \
  -H "Content-Type: application/json" \
  -H "CorrelationId: your-correlation-id" \
  -H "Authorization: Bearer your-jwt-token" \
  -d '{
    "routeId": "auth-route",
    "path": "/auth/{**catch-all}",
    "methods": ["GET", "POST"],
    "rateLimiterPolicy": "blogsphereratelimitter",
    "clusterId": "auth-service-cluster-id",
    "headers": [
      {
        "name": "X-Custom-Header",
        "values": ["value1", "value2"],
        "mode": "Append"
      }
    ],
    "transforms": [
      {
        "pathPattern": "/transformed/{**catch-all}"
      }
    ]
  }'

# 3. Refresh configuration
curl -X POST http://localhost:8000/api/v1/proxyconfiguration/refresh \
  -H "CorrelationId: your-correlation-id" \
  -H "Authorization: Bearer your-jwt-token"

# 4. Create an API product
curl -X POST http://localhost:8000/api/v1/apiproduct \
  -H "Content-Type: application/json" \
  -H "CorrelationId: your-correlation-id" \
  -H "Authorization: Bearer your-jwt-token" \
  -d '{
    "name": "Auth API Product",
    "description": "Authentication and authorization services"
  }'
```

### Response Formats

#### Success Response

```json
{
  "isSuccess": true,
  "value": {
    // Response data
  },
  "error": null
}
```

#### Validation Error Response

```json
{
  "isSuccess": false,
  "error": {
    "code": "ValidationError",
    "message": "One or more validation errors occurred",
    "details": [
      {
        "field": "fieldName",
        "message": "Error message"
      }
    ]
  }
}
```

#### Not Found Response

```json
{
  "isSuccess": false,
  "error": {
    "code": "NotFound",
    "message": "Resource not found"
  }
}
```

#### Internal Server Error Response

```json
{
  "isSuccess": false,
  "error": {
    "code": "InternalServerError",
    "message": "An unexpected error occurred"
  }
}
```

## Docker Support

### Build the Image

```bash
docker build -t blogsphere-gateway .
```

### Run the Container

```bash
docker run -d \
  -p 8000:80 \
  -e ConnectionStrings__DefaultConnection="Server=host.docker.internal;Database=BlogsphereProxy;Username=admin;Password=admin" \
  -e EventBus__Host="host.docker.internal" \
  -e Elasticsearch__Uri="http://host.docker.internal:9200" \
  -e Zipkin__Url="http://host.docker.internal:9411/api/v2/spans" \
  blogsphere-gateway
```

## Monitoring and Telemetry

### Logging

- Uses Serilog for structured logging
- Supports console and Elasticsearch outputs
- Includes correlation IDs and method tracing

### OpenTelemetry

- Integrated with Zipkin for distributed tracing
- Tracks HTTP requests and responses
- Monitors database operations

### Health Checks

- Database connectivity
- Dependent service status
- Resource utilization

## Security

### Built-in Security Headers

```csharp
X-Content-Type-Options: nosniff
X-Frame-Options: DENY
X-XSS-Protection: 1; mode=block
Referrer-Policy: strict-origin-when-cross-origin
Content-Security-Policy: default-src 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline';
```

### Authentication & Authorization

- JWT Bearer token authentication with configurable authority
- Role-based access control with `[RequirePermission]` attribute
- API scopes for fine-grained access control
- Subscription validation middleware
- Identity service integration for user management

## Development

### Project Structure

```
src/
├── Blogsphere.Api.Gateway/
    ├── Configurations/     # Application configuration options
    ├── Controllers/        # API endpoints (v1 versioned)
    ├── Data/              # Database context, repositories, and migrations
    │   ├── Configurations/ # Entity configurations
    │   ├── Context/        # DbContext
    │   ├── Interfaces/     # Repository interfaces
    │   ├── Migrations/     # EF Core migrations
    │   ├── Repositories/   # Repository implementations
    │   ├── Seeding/        # Database seeding
    │   └── UnitOfWork/     # Unit of work pattern
    ├── DI/                # Dependency injection extensions
    ├── Entity/            # Domain entities
    ├── EventBus/          # Event bus contracts and implementations
    ├── Extensions/        # Extension methods
    ├── Filters/           # Custom filters (permissions, etc.)
    ├── Infrastructure/    # Cross-cutting concerns and YARP integration
    ├── Middlewares/       # Custom middleware
    ├── Models/            # DTOs, requests, responses, and mappings
    │   ├── Common/        # Common response models
    │   ├── Constants/     # Application constants
    │   ├── DTOs/          # Data transfer objects
    │   ├── Enums/         # Enumerations
    │   ├── Mappings/      # AutoMapper profiles
    │   └── Requests/      # Request models
    ├── Services/          # Business logic services
    │   ├── Factory/       # Service factories
    │   ├── Interfaces/    # Service interfaces
    │   └── Security/      # Security services
    ├── Swagger/           # Swagger/OpenAPI configuration and examples
    └── Validators/        # FluentValidation validators
```

### Adding New Features

#### Adding New API Endpoints

1. Create controller in `Controllers/v1/`
2. Add Swagger documentation with `[SwaggerOperation]` and `[SwaggerHeader]` attributes
3. Implement business logic in services
4. Add request/response models in `Models/`
5. Add validation using FluentValidation
6. Add AutoMapper profiles for object mapping

#### Adding New Proxy Routes

1. Create a cluster configuration via API
2. Define route patterns and methods
3. Configure rate limiting if needed
4. Add any required transforms
5. Test the routing behavior

#### Adding New Subscriptions

1. Create API products via the API product endpoints
2. Set up subscribed APIs for specific products
3. Create subscriptions linking users to API products
4. Configure access permissions and rate limits

### Best Practices

1. **API Design**:

   - Always include `[SwaggerHeader("CorrelationId")]` on all endpoints [[memory:2839113]]
   - Use `[SwaggerOperation]` and `[SwaggerRequestExample]` attributes for documentation [[memory:2839113]]
   - Follow RESTful conventions for endpoint naming
   - Use proper HTTP status codes

2. **Configuration**:

   - Use meaningful IDs for clusters and routes
   - Keep configurations focused and simple
   - Document any special routing requirements
   - Use environment-specific configuration files

3. **Security**:

   - Always use HTTPS in production
   - Implement appropriate rate limiting
   - Validate and sanitize inputs
   - Use JWT authentication for protected endpoints
   - Implement role-based access control

4. **Performance**:

   - Monitor route performance
   - Use caching where appropriate
   - Implement circuit breakers for critical services
   - Configure appropriate connection pooling

5. **Development**:
   - Use the repository pattern for data access
   - Implement proper logging with correlation IDs
   - Write comprehensive unit and integration tests
   - Use dependency injection for loose coupling

## Troubleshooting

### Common Issues

1. **Database Connection**:

   - Verify connection string
   - Check PostgreSQL service status
   - Ensure database migrations are up to date

2. **Routing Problems**:

   - Validate cluster and route configurations
   - Check destination service availability
   - Review YARP logs for routing decisions

3. **Performance Issues**:

   - Monitor rate limit counters
   - Check resource utilization
   - Review telemetry data
   - Verify Event Bus connectivity (RabbitMQ)

4. **Authentication Issues**:

   - Verify JWT token validity
   - Check identity authority configuration
   - Ensure proper permissions are assigned
   - Validate subscription status

5. **Documentation Issues**:
   - Access Scalar UI at `/scalar/v1` for modern API documentation
   - Check Swagger UI at `/swagger` for traditional documentation
   - Verify correlation IDs in request headers

## CI/CD Pipeline

The project uses Azure DevOps pipelines with the following stages:

1. **Build**: Compiles the .NET project using `dotnet build` [[memory:2839082]]
2. **Code Analysis**: Runs static code analysis
3. **GitHub Release**: Creates releases for main branch builds
4. **Docker Build and Push**: Builds and pushes Docker images

### Pipeline Configuration

The pipeline is triggered on:

- **Branches**: `main`, `feature/**`, `hotfix/**`
- **Pull Requests**: targeting `main` branch

### Build Requirements

- .NET 8.0 SDK
- Ubuntu latest build agents
- Docker for containerization

## Contributing

1. Fork the repository
2. Create a feature branch (`feature/your-feature-name`)
3. Implement your changes following the coding standards
4. Add appropriate tests and documentation
5. Ensure all CI checks pass
6. Commit your changes with descriptive messages
7. Push to your branch
8. Create a Pull Request with detailed description

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

For support, contact the Blogsphere team at support@blogsphere.com or open an issue in the repository.

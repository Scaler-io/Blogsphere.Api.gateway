# Blogsphere API Gateway

A dynamic, configurable API Gateway built with .NET 8 and YARP (Yet Another Reverse Proxy) for the Blogsphere microservices architecture.

## Features

- 🚀 Dynamic routing configuration via REST API
- 🔒 Built-in security features and headers
- 📊 Rate limiting and request throttling
- 🔄 Real-time configuration updates
- 📝 Comprehensive logging with Serilog
- 📈 OpenTelemetry integration with Zipkin
- 🗄️ PostgreSQL for configuration storage
- 🔍 Swagger/OpenAPI documentation
- 🎯 Health check endpoints
- 🌐 CORS support

## Prerequisites

- .NET 8.0 SDK
- PostgreSQL 13+
- Docker (optional)
- Git

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/your-org/blogsphere-api-gateway.git
cd blogsphere-api-gateway
```

### 2. Configure Database

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=blogsphere;Username=your_username;Password=your_password"
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

- API: http://localhost:5000
- Swagger UI: http://localhost:5000/swagger

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
http://localhost:5000/api/v1
```

### Key Endpoints

#### Clusters Management

```http
GET    /clusters            # List all clusters
GET    /clusters/{id}       # Get cluster by ID
POST   /clusters           # Create cluster
PUT    /clusters/{id}      # Update cluster
DELETE /clusters/{id}      # Delete cluster
```

Request Headers:

- `CorrelationId`: Required for request tracing
- `Content-Type`: application/json

#### Routes Management

```http
GET    /routes            # List all routes
GET    /routes/{id}       # Get route by ID
POST   /routes           # Create route
PUT    /routes/{id}      # Update route
DELETE /routes/{id}      # Delete route
```

Request Headers:

- `CorrelationId`: Required for request tracing
- `Content-Type`: application/json

#### Configuration Management

```http
POST   /configuration/refresh     # Force configuration refresh
```

Request Headers:

- `CorrelationId`: Required for request tracing

### Example: Creating a Route

```bash
# 1. Create a cluster
curl -X POST http://localhost:5000/api/v1/clusters \
  -H "Content-Type: application/json" \
  -H "CorrelationId: your-correlation-id" \
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
curl -X POST http://localhost:5000/api/v1/routes \
  -H "Content-Type: application/json" \
  -H "CorrelationId: your-correlation-id" \
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
curl -X POST http://localhost:5000/api/v1/configuration/refresh \
  -H "CorrelationId: your-correlation-id"
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
  -p 5000:80 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Database=blogsphere;Username=your_username;Password=your_password" \
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

### Authentication

- JWT Bearer token authentication
- Role-based access control
- Subscription validation middleware

## Development

### Project Structure

```
src/
├── Blogsphere.Api.Gateway/
    ├── Controllers/         # API endpoints
    ├── Data/               # Database context and repositories
    ├── Entity/             # Domain entities
    ├── Extensions/         # Extension methods
    ├── Infrastructure/     # Cross-cutting concerns
    ├── Middlewares/        # Custom middleware
    ├── Models/             # DTOs and view models
    └── Services/           # Business logic
```

### Adding New Routes

1. Create a cluster configuration
2. Define route patterns and methods
3. Configure rate limiting if needed
4. Add any required transforms
5. Test the routing behavior

### Best Practices

1. **Configuration**:

   - Use meaningful IDs for clusters and routes
   - Keep configurations focused and simple
   - Document any special routing requirements

2. **Security**:

   - Always use HTTPS in production
   - Implement appropriate rate limiting
   - Validate and sanitize inputs

3. **Performance**:
   - Monitor route performance
   - Use caching where appropriate
   - Implement circuit breakers for critical services

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

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

For support, contact the Blogsphere team at support@blogsphere.com or open an issue in the repository.

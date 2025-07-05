# Blogsphere API Gateway

A modern API Gateway built with YARP (Yet Another Reverse Proxy) to secure and manage access to Blogsphere microservices.

## Overview

This API Gateway serves as the entry point for all client-side requests to the Blogsphere ecosystem. It provides essential features such as:

- Authentication and Authorization
- Request routing and load balancing
- Rate limiting
- Request/Response transformation
- Monitoring and logging
- SSL termination

## Technology Stack

- .NET 8
- YARP (Yet Another Reverse Proxy)
- Serilog for structured logging
- JWT Bearer Authentication
- Elasticsearch for log aggregation

## Prerequisites

- .NET SDK 8.0.100 or later
- Docker (optional, for containerization)

## Getting Started

1. Clone the repository:

   ```bash
   git clone https://github.com/yourusername/Blogsphere.Api.Gateway.git
   cd Blogsphere.Api.Gateway
   ```

2. Build the solution:

   ```bash
   dotnet build
   ```

3. Run the application:
   ```bash
   dotnet run --project src/Blogsphere.Api.Gateway
   ```

## Configuration

The gateway can be configured through the following files:

- `appsettings.json` - Default configuration
- `appsettings.Development.json` - Development environment settings
- `appsettings.Docker.json` - Docker environment settings

### Route Configuration

Routes are defined in the configuration file under the `ReverseProxy` section. Example:

```json
{
  "ReverseProxy": {
    "Routes": {
      "posts-route": {
        "ClusterId": "posts-cluster",
        "Match": {
          "Path": "/posts/{**catch-all}"
        }
      }
    },
    "Clusters": {
      "posts-cluster": {
        "Destinations": {
          "posts-api": {
            "Address": "http://posts-api:5000"
          }
        }
      }
    }
  }
}
```

## Security

The gateway implements several security measures:

- JWT Authentication
- HTTPS redirection
- Rate limiting
- IP filtering (optional)

## Logging

Logging is implemented using Serilog with the following sinks:

- Console (for development)
- Elasticsearch (for production)

## Monitoring

The application includes:

- Health checks endpoint
- Telemetry using OpenTelemetry
- Performance metrics

## Docker Support

Build the Docker image:

```bash
docker build -t blogsphere-gateway .
```

Run the container:

```bash
docker run -p 5000:80 blogsphere-gateway
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

For support, please open an issue in the GitHub repository.

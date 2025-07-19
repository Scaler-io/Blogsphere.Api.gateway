FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Blogsphere.Api.Gateway/Blogsphere.Api.Gateway.csproj", "Blogsphere.Api.Gateway/"]
RUN dotnet restore "./Blogsphere.Api.Gateway/Blogsphere.Api.Gateway.csproj"
COPY . .
WORKDIR "/src/Blogsphere.Api.Gateway"
RUN dotnet build "./Blogsphere.Api.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Blogsphere.Api.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Blogsphere.Api.Gateway.dll"]
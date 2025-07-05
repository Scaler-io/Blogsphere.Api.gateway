using Blogsphere.Api.Gateway;
using Blogsphere.Api.Gateway.Configurations;
using Blogsphere.Api.Gateway.Data.Context;
using Blogsphere.Api.Gateway.Data.Interfaces;
using Blogsphere.Api.Gateway.Data.Interfaces.Repositories;
using Blogsphere.Api.Gateway.Data.Repositories;
using Blogsphere.Api.Gateway.Data.Seeding;
using Blogsphere.Api.Gateway.Data.UnitOfWork;
using Blogsphere.Api.Gateway.Infrastructure.BackgroundServices;
using Blogsphere.Api.Gateway.Infrastructure.Extensions;
using Blogsphere.Api.Gateway.Middlewares;
using Blogsphere.Api.Gateway.Services;
using Blogsphere.Api.Gateway.Services.Interfaces;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

var appConfigOption = builder.Configuration
.GetSection(AppConfigOption.OptionName)
.Get<AppConfigOption>();

var identityGroupAccess = builder.Configuration
.GetSection(IdentityGroupAccessOption.OptionName)
.Get<IdentityGroupAccessOption>();

var logger = Logging.CreateLogger(builder.Configuration);

builder.Services.AddSingleton(logger);
builder.Host.UseSerilog(logger);

// Add database configuration
builder.Services.AddDbContext<ProxyConfigContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        o =>
        {
            o.MigrationsAssembly("Blogsphere.Api.Gateway");
            o.MigrationsHistoryTable("__EFMigrationsHistory", "blogsphere");
        })
);

// Register repositories
builder.Services.AddScoped<IProxyRouteRepository, ProxyRouteRepository>();
builder.Services.AddScoped<IProxyClusterRepository, ProxyClusterRepository>();
builder.Services.AddScoped<IProxyDestinationRepository, ProxyDestinationRepository>();
builder.Services.AddScoped<IProxyHeaderRepository, ProxyHeaderRepository>();
builder.Services.AddScoped<IProxyTransformRepository, ProxyTransformRepository>();

// Register Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Services
builder.Services.AddScoped<IProxyRouteService, ProxyRouteService>();
builder.Services.AddScoped<IProxyClusterService, ProxyClusterService>();
builder.Services.AddScoped<IProxyDestinationService, ProxyDestinationService>();

// Register Seeder
builder.Services.AddProxyConfigSeeder();

builder.Services.AddRateLimiter(options => 
    options.AddFixedWindowLimiter("blogsphereratelimitter", policy => {
        policy.PermitLimit = 4;
        policy.Window = TimeSpan.FromSeconds(12);
        policy.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        policy.QueueLimit = 1;
    }));

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
        tracing.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(appConfigOption.ApplicationIdentifier))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddZipkinExporter(options => options.Endpoint = new Uri(builder.Configuration["Zipkin:Url"]))
    );

// Configure YARP with database-driven configuration
builder.Services.AddReverseProxy()
    .AddDatabaseConfig();

// Register background service for config refresh
builder.Services.AddHostedService<ProxyConfigRefreshService>();

builder.Services.AddTransient<ISubscriptionService, SubscriptionService>();
builder.Services.AddTransient<SubscriptionValidationMiddleware>();
builder.Services.AddCors(options => options.AddPolicy("blogspherecors", policy => policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

app.UseCors("blogspherecors");
app.UseMiddleware<SubscriptionValidationMiddleware>();
app.UseRateLimiter();
app.MapReverseProxy();

// Run the seeder
await app.Services.GetRequiredService<IHost>().SeedProxyConfigurationAsync();

app.Run();


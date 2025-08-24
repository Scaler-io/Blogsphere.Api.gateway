using Asp.Versioning.ApiExplorer;
using Scalar.AspNetCore;

namespace Blogsphere.Api.Gateway.DI;

public static class ApplicationPipelineExtensions
{
    public static async Task<WebApplication> UseApplicationPipelineAsync(this WebApplication app){
        app.UseSwagger(SwaggerConfiguration.SetupSwaggerOptions);
        app.UseSwaggerUI(options =>
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            SwaggerConfiguration.SetupSwaggerUiOptions(options, provider);
            foreach(var description in provider.ApiVersionDescriptions){
               app.MapScalarApiReference($"scalar/{description.GroupName}", options => {
                    SwaggerConfiguration.SetupScalarUiOptions(options, description);
               }).WithMetadata(new SkipSubscriptionValidationAttribute());
            }
        });

        // Configure security headers
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
            context.Response.Headers.Append("Content-Security-Policy", 
                "default-src 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline';");
            await next();
        });

        app.UseCors("blogspherecors");

        // Apply subscription validation middleware globally
        app.UseMiddleware<CorrelationHeaderEnricher>();
        app.UseMiddleware<RequestLoggerMiddleware>();
        app.UseMiddleware<SubscriptionValidationMiddleware>();
        app.UseMiddleware<GlobalExceptionMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();

        // Skip subscription validation for API endpoints
        app.MapControllers()
            .WithMetadata(new SkipSubscriptionValidationAttribute());

        app.UseRateLimiter();
        app.MapReverseProxy();

        // Run the seeder
        await app.Services.GetRequiredService<IHost>()
            .SeedProxyConfigurationAsync();
            
        return app;
    }
}

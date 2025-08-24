using FluentValidation;

namespace Blogsphere.Api.Gateway.DI;

public static class ValidatorExtensions
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddTransient<IValidator<CreateProxyClusterRequest>, ProxyClusterRequestValidators>();

        return services;
    }
}

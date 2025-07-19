using FluentValidation;

namespace Blogsphere.Api.Gateway.Validators;

public class ProxyClusterRequestValidators : AbstractValidator<CreateProxyClusterRequest>
{
    public ProxyClusterRequestValidators()
    {
        RuleFor(x => x.ClusterId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode("1001")
            .WithMessage("Please provide a valid cluster id");
        
        RuleFor(x => x.Destinations)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode("1002")
            .WithMessage("Please provide at least one destination");
    }
}

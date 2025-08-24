using Swashbuckle.AspNetCore.Filters;

namespace Blogsphere.Api.Gateway.Swagger.Examples;

public class UpdateSubscribedApiRequestExample : IExamplesProvider<UpdateSubscribedApiRequest>
{
    public UpdateSubscribedApiRequest GetExamples()
    {
        return new UpdateSubscribedApiRequest
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid()
        };
    }
}

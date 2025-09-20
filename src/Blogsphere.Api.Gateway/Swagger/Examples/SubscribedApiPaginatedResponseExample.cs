using Swashbuckle.AspNetCore.Filters;

namespace Blogsphere.Api.Gateway.Swagger.Examples;

public class SubscribedApiPaginatedResponseExample : IExamplesProvider<PaginatedResult<SubscribedApiDto>>
{
    public PaginatedResult<SubscribedApiDto> GetExamples()
    {
        return new PaginatedResult<SubscribedApiDto>
        {
            Items = [
                new SubscribedApiDto
                {
                    Id = Guid.NewGuid(),
                    ApiName = "TestApi 1",
                    ApiPath = "/test",
                    ApiDescription = "Test Api Description 1",
                },
                new SubscribedApiDto
                {
                    Id = Guid.NewGuid(),
                    ApiName = "TestApi 2",
                    ApiPath = "/test",
                    ApiDescription = "Test Api Description 2",
                }
            ],
            TotalCount = 10,
            PageNumber = 1,
            PageSize = 10,
        };
    }
}

public class SubscribedApiListResponseExample : IExamplesProvider<List<SubscribedApiDto>>
{
    public List<SubscribedApiDto> GetExamples()
    {
        return [
            new SubscribedApiDto
            {
                Id = Guid.NewGuid(),
                ApiName = "TestApi 1",
                ApiPath = "/test",
                ApiDescription = "Test Api Description 1",
            },
            new SubscribedApiDto
            {
                Id = Guid.NewGuid(),
                ApiName = "TestApi 2",
                ApiPath = "/test",
                ApiDescription = "Test Api Description 2",
            }
        ];
    }
}

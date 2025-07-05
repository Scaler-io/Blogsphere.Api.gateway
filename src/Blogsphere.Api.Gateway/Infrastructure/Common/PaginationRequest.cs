namespace Blogsphere.Api.Gateway.Infrastructure.Common;

public class PaginationRequest
{
    private const int MaxPageSize = 50;
    private int _pageSize = 10;
    private int _pageNumber = 1;

    public string SearchTerm { get; init; } = string.Empty;
    public string SortBy { get; init; } = string.Empty;
    public bool IsDescending { get; init; }

    public int PageNumber
    {
        get => _pageNumber;
        init => _pageNumber = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value > MaxPageSize ? MaxPageSize : value < 1 ? 1 : value;
    }
} 
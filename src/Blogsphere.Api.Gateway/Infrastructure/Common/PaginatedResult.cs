namespace Blogsphere.Api.Gateway.Infrastructure.Common;

public class PaginatedResult<T>(IReadOnlyList<T> items, int pageNumber, int pageSize, int totalCount)
{
    public IReadOnlyList<T> Items { get; } = items;
    public int PageNumber { get; } = pageNumber;
    public int PageSize { get; } = pageSize;
    public int TotalCount { get; } = totalCount;
    public int TotalPages { get; } = (int)Math.Ceiling(totalCount / (double)pageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
} 
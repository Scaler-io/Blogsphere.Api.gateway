namespace Blogsphere.Api.Gateway.Models.Common;

public class PaginationRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SearchTerm { get; set; }
    public string SortBy { get; set; }
    public bool IsDescending { get; set; }
    public bool? IsActive { get; set; } // null means return both active and inactive
} 
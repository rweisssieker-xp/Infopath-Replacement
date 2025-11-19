namespace FormBuilderService.Models.DTOs;

/// <summary>
/// Request DTO for pagination parameters
/// </summary>
public class PaginationRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; } = "asc"; // "asc" or "desc"
}


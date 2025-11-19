namespace AuthService.Models.DTOs;

/// <summary>
/// Standard error response DTO
/// </summary>
public class ErrorResponse
{
    public string Error { get; set; } = string.Empty;
    public string? Message { get; set; }
    public string? CorrelationId { get; set; }
}


using System.Net.Http.Json;
using System.Text.Json;
using IntegrationGatewayService.GraphQL.Inputs;
using IntegrationGatewayService.GraphQL.Types;
using Microsoft.Extensions.Options;

namespace IntegrationGatewayService.Services;

/// <summary>
/// HTTP client for communicating with Form Builder Service
/// </summary>
public class FormServiceClient : IFormServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FormServiceClient> _logger;

    public FormServiceClient(HttpClient httpClient, ILogger<FormServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<FormType>> GetFormsAsync(int pageNumber = 1, int pageSize = 10, string? nameFilter = null, FormStatus? statusFilter = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var queryParams = new List<string>
            {
                $"pageNumber={pageNumber}",
                $"pageSize={pageSize}"
            };

            if (!string.IsNullOrEmpty(nameFilter))
            {
                queryParams.Add($"name={Uri.EscapeDataString(nameFilter)}");
            }

            if (statusFilter.HasValue)
            {
                queryParams.Add($"status={statusFilter.Value}");
            }

            var queryString = string.Join("&", queryParams);
            var response = await _httpClient.GetAsync($"/api/forms?{queryString}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var paginatedResponse = await response.Content.ReadFromJsonAsync<PaginatedFormResponse>(cancellationToken: cancellationToken);
            return paginatedResponse?.Items.Select(MapToFormType).ToList() ?? new List<FormType>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching forms from Form Builder Service");
            throw;
        }
    }

    public async Task<FormType?> GetFormByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/forms/{id}", cancellationToken);
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            var formResponse = await response.Content.ReadFromJsonAsync<FormResponseDto>(cancellationToken: cancellationToken);
            return formResponse != null ? MapToFormType(formResponse) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching form {FormId} from Form Builder Service", id);
            throw;
        }
    }

    public async Task<FormType> CreateFormAsync(CreateFormInput input, Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new
            {
                Name = input.Name,
                Description = input.Description,
                Schema = JsonSerializer.Deserialize<Dictionary<string, object>>(input.Schema.GetRawText())
            };

            var response = await _httpClient.PostAsJsonAsync("/api/forms", request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var formResponse = await response.Content.ReadFromJsonAsync<FormResponseDto>(cancellationToken: cancellationToken);
            return MapToFormType(formResponse!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating form in Form Builder Service");
            throw;
        }
    }

    public async Task<FormType> UpdateFormAsync(Guid id, UpdateFormInput input, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new
            {
                Name = input.Name,
                Description = input.Description,
                Schema = input.Schema.HasValue ? JsonSerializer.Deserialize<Dictionary<string, object>>(input.Schema.Value.GetRawText()) : null,
                Status = input.Status?.ToString()
            };

            var response = await _httpClient.PutAsJsonAsync($"/api/forms/{id}", request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var formResponse = await response.Content.ReadFromJsonAsync<FormResponseDto>(cancellationToken: cancellationToken);
            return MapToFormType(formResponse!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating form {FormId} in Form Builder Service", id);
            throw;
        }
    }

    public async Task<bool> DeleteFormAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/forms/{id}", cancellationToken);
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }

            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting form {FormId} in Form Builder Service", id);
            throw;
        }
    }

    private static FormType MapToFormType(FormResponseDto dto)
    {
        return new FormType
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            Schema = JsonSerializer.SerializeToElement(dto.Schema),
            Version = dto.Version,
            Status = Enum.Parse<FormStatus>(dto.Status, ignoreCase: true),
            CreatedBy = dto.CreatedBy,
            TenantId = dto.TenantId,
            Tags = dto.Tags ?? new List<string>(),
            Category = dto.Category,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
    }

    private class FormResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Dictionary<string, object> Schema { get; set; } = new();
        public string Version { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Guid CreatedBy { get; set; }
        public Guid TenantId { get; set; }
        public List<string>? Tags { get; set; }
        public string? Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    private class PaginatedFormResponse
    {
        public List<FormResponseDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}


using HotChocolate;
using HotChocolate.Types;
using System.Text.Json;

namespace IntegrationGatewayService.GraphQL.Types;

/// <summary>
/// GraphQL type representing a Form entity
/// </summary>
public class FormType
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public JsonElement Schema { get; set; }
    public string Version { get; set; } = "1.0.0";
    public FormStatus Status { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid TenantId { get; set; }
    public List<string> Tags { get; set; } = new();
    public string? Category { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Form status enumeration for GraphQL
/// </summary>
public enum FormStatus
{
    DRAFT = 1,
    PUBLISHED = 2,
    ARCHIVED = 3
}


namespace IntegrationGatewayService.GraphQL.Types;

/// <summary>
/// GraphQL type representing a User entity
/// </summary>
public class UserType
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public Guid TenantId { get; set; }
}


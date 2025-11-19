using HotChocolate;
using System.Text.Json;

namespace IntegrationGatewayService.GraphQL.Inputs;

/// <summary>
/// Input type for creating a new form
/// </summary>
public class CreateFormInput
{
    [GraphQLNonNullType]
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [GraphQLNonNullType]
    public JsonElement Schema { get; set; }
    
    public string? Category { get; set; }
    
    public List<string> Tags { get; set; } = new();
}


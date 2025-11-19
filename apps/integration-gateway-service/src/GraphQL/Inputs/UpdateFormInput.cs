using HotChocolate;
using IntegrationGatewayService.GraphQL.Types;
using System.Text.Json;

namespace IntegrationGatewayService.GraphQL.Inputs;

/// <summary>
/// Input type for updating an existing form
/// </summary>
public class UpdateFormInput
{
    public string? Name { get; set; }
    
    public string? Description { get; set; }
    
    public JsonElement? Schema { get; set; }
    
    public FormStatus? Status { get; set; }
    
    public string? Category { get; set; }
    
    public List<string>? Tags { get; set; }
}


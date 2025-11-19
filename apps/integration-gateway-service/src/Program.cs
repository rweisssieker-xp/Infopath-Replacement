using HotChocolate.AspNetCore;
using IntegrationGatewayService.GraphQL.Mutations;
using IntegrationGatewayService.GraphQL.Queries;
using IntegrationGatewayService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add HTTP context accessor for extracting JWT tokens (must be added before HTTP clients)
builder.Services.AddHttpContextAccessor();

// Configure HTTP clients for backend services with JWT forwarding
builder.Services.AddHttpClient<IFormServiceClient, FormServiceClient>(client =>
{
    var baseUrl = builder.Configuration["FormBuilderService:BaseUrl"] ?? "http://localhost:5001";
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<IntegrationGatewayService.Handlers.JwtForwardingHandler>();

builder.Services.AddHttpClient<IAuthServiceClient, AuthServiceClient>(client =>
{
    var baseUrl = builder.Configuration["AuthService:BaseUrl"] ?? "http://localhost:5000";
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<IntegrationGatewayService.Handlers.JwtForwardingHandler>();

// Register the JWT forwarding handler
builder.Services.AddTransient<IntegrationGatewayService.Handlers.JwtForwardingHandler>();

// Configure JWT Authentication
var jwtSecret = builder.Configuration["JWT:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
var jwtIssuer = builder.Configuration["JWT:Issuer"] ?? "FormXChangeAuthService";
var jwtAudience = builder.Configuration["JWT:Audience"] ?? "FormXChangeClients";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Configure Hot Chocolate GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType(d => d.Name("Query"))
        .AddTypeExtension<FormQueries>()
        .AddTypeExtension<UserQueries>()
    .AddMutationType(d => d.Name("Mutation"))
        .AddTypeExtension<FormMutations>()
    .AddType<IntegrationGatewayService.GraphQL.Types.FormType>()
    .AddType<IntegrationGatewayService.GraphQL.Types.UserType>()
    .AddType<IntegrationGatewayService.GraphQL.Types.FormStatus>()
    .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = builder.Environment.IsDevelopment());

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL();
app.MapControllers();

app.Run();

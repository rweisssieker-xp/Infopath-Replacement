using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using FormBuilderService.Data;
using FormBuilderService.Repositories;
using FormBuilderService.Services;
using FormBuilderService.Validators;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Form Builder Service API",
        Version = "v1",
        Description = "API for managing form definitions and metadata"
    });
});

// Configure database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<FormBuilderDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configure JWT Authentication
var jwtSecret = builder.Configuration["JWT:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
var jwtIssuer = builder.Configuration["JWT:Issuer"] ?? "FormXChange.AuthService";
var jwtAudience = builder.Configuration["JWT:Audience"] ?? "FormXChange.Suite";

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

// Configure FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateFormRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// Register repositories
builder.Services.AddScoped<IFormRepository, FormRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();

// Register services
builder.Services.AddScoped<IFormService, FormService>();

// Add HttpContextAccessor for services
builder.Services.AddHttpContextAccessor();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(builder.Configuration["AllowedOrigins"]?.Split(",") ?? new[] { "http://localhost:3000" })
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Ensure database is created (for development only)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<FormBuilderDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AuthService.Configuration;
using AuthService.Data;
using AuthService.Repositories;
using AuthService.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configure authentication configuration
builder.Services.Configure<AuthConfiguration>(
    builder.Configuration.GetSection(AuthConfiguration.SectionName));

var authConfig = builder.Configuration.GetSection(AuthConfiguration.SectionName).Get<AuthConfiguration>() 
    ?? throw new InvalidOperationException("Auth configuration not found");

// Configure Azure AD Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "AzureAD";
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authConfig.Jwt.Secret)),
        ValidateIssuer = true,
        ValidIssuer = authConfig.Jwt.Issuer,
        ValidateAudience = true,
        ValidAudience = authConfig.Jwt.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
})
.AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"), "AzureAD");

// Configure authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("FormBuilder", policy => policy.RequireRole("FormBuilder", "Admin"));
    options.AddPolicy("FormUser", policy => policy.RequireRole("FormUser", "FormBuilder", "Admin"));
    options.AddPolicy("Viewer", policy => policy.RequireRole("Viewer", "FormUser", "FormBuilder", "Admin"));
});

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

// Register services
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IUserService, UserService>();

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
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();

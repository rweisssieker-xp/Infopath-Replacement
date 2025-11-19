namespace AuthService.Configuration;

/// <summary>
/// Authentication configuration settings
/// </summary>
public class AuthConfiguration
{
    public const string SectionName = "Auth";

    public JwtSettings Jwt { get; set; } = new();
    public AzureAdSettings AzureAd { get; set; } = new();
}

public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; } = 15;
    public int RefreshTokenExpirationDays { get; set; } = 7;
}

public class AzureAdSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string Instance { get; set; } = "https://login.microsoftonline.com/";
    public string CallbackPath { get; set; } = "/api/auth/callback";
}


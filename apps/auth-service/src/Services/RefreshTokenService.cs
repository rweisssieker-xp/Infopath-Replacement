using System.Security.Cryptography;
using AuthService.Configuration;
using AuthService.Models;
using AuthService.Repositories;
using Microsoft.Extensions.Options;

namespace AuthService.Services;

/// <summary>
/// Refresh token service implementation
/// </summary>
public class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly JwtSettings _jwtSettings;

    public RefreshTokenService(
        IRefreshTokenRepository refreshTokenRepository,
        IOptions<AuthConfiguration> authConfig)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _jwtSettings = authConfig.Value.Jwt;
    }

    public async Task<RefreshToken> GenerateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var token = GenerateSecureRandomToken();
        var refreshToken = new RefreshToken
        {
            Token = token,
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
        };

        return await _refreshTokenRepository.CreateAsync(refreshToken, cancellationToken);
    }

    public async Task<RefreshToken?> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(token, cancellationToken);
        
        if (refreshToken == null || !refreshToken.IsActive)
        {
            return null;
        }

        return refreshToken;
    }

    public async Task RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(token, cancellationToken);
        if (refreshToken != null)
        {
            await _refreshTokenRepository.RevokeAsync(refreshToken.Id, cancellationToken);
        }
    }

    public async Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await _refreshTokenRepository.RevokeAllForUserAsync(userId, cancellationToken);
    }

    private static string GenerateSecureRandomToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}


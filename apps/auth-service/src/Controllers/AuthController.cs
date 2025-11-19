using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AuthService.Models.DTOs;
using AuthService.Services;

namespace AuthService.Controllers;

/// <summary>
/// Authentication controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUserService userService,
        IJwtTokenService jwtTokenService,
        IRefreshTokenService refreshTokenService,
        ILogger<AuthController> logger)
    {
        _userService = userService;
        _jwtTokenService = jwtTokenService;
        _refreshTokenService = refreshTokenService;
        _logger = logger;
    }

    /// <summary>
    /// Initiate OAuth2/OIDC login (redirects to Azure AD)
    /// </summary>
    [HttpPost("login")]
    public IActionResult Login([FromQuery] string? returnUrl = null)
    {
        // Azure AD authentication is handled by Microsoft.Identity.Web middleware
        // This endpoint initiates the challenge
        var redirectUrl = returnUrl ?? "/";
        return Challenge(new Microsoft.AspNetCore.Authentication.AuthenticationProperties
        {
            RedirectUri = redirectUrl
        }, "AzureAD");
    }

    /// <summary>
    /// OAuth2 callback handler (handled by Azure AD middleware)
    /// </summary>
    [HttpGet("callback")]
    public IActionResult Callback()
    {
        // This is handled by Microsoft.Identity.Web middleware
        // After successful authentication, redirect to frontend with tokens
        return Redirect("/");
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var refreshToken = await _refreshTokenService.ValidateRefreshTokenAsync(request.RefreshToken);
            if (refreshToken == null || refreshToken.User == null)
            {
                return Unauthorized(new ErrorResponse
                {
                    Error = "invalid_refresh_token",
                    Message = "Invalid or expired refresh token"
                });
            }

            // Generate new access token
            var accessToken = _jwtTokenService.GenerateAccessToken(refreshToken.User);

            // Rotate refresh token (revoke old, create new)
            await _refreshTokenService.RevokeRefreshTokenAsync(request.RefreshToken);
            var newRefreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(refreshToken.UserId);

            return Ok(new RefreshTokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken.Token,
                ExpiresIn = 900 // 15 minutes in seconds
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return StatusCode(500, new ErrorResponse
            {
                Error = "internal_error",
                Message = "An error occurred while refreshing the token"
            });
        }
    }

    /// <summary>
    /// Logout user and revoke refresh tokens
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                await _refreshTokenService.RevokeAllUserTokensAsync(userId);
            }

            // Sign out from Azure AD
            await HttpContext.SignOutAsync("AzureAD");
            await HttpContext.SignOutAsync();

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, new ErrorResponse
            {
                Error = "internal_error",
                Message = "An error occurred during logout"
            });
        }
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(new ErrorResponse
                {
                    Error = "invalid_token",
                    Message = "Invalid user identifier in token"
                });
            }

            var userProfile = await _userService.GetUserProfileAsync(userId);
            if (userProfile == null)
            {
                return NotFound(new ErrorResponse
                {
                    Error = "user_not_found",
                    Message = "User not found"
                });
            }

            return Ok(userProfile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user profile");
            return StatusCode(500, new ErrorResponse
            {
                Error = "internal_error",
                Message = "An error occurred while retrieving user profile"
            });
        }
    }

    /// <summary>
    /// Get user permissions
    /// </summary>
    [HttpGet("permissions")]
    [Authorize]
    public async Task<IActionResult> GetPermissions()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(new ErrorResponse
                {
                    Error = "invalid_token",
                    Message = "Invalid user identifier in token"
                });
            }

            var permissions = await _userService.GetUserPermissionsAsync(userId);
            return Ok(new PermissionsResponse
            {
                Permissions = permissions
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user permissions");
            return StatusCode(500, new ErrorResponse
            {
                Error = "internal_error",
                Message = "An error occurred while retrieving permissions"
            });
        }
    }
}


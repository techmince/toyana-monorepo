using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Toyana.Identity.Models;

namespace Toyana.Identity.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration        _config;
    private readonly ILogger<TokenService> _logger;
    private readonly ISessionManager       _sessionManager;

    public TokenService(IConfiguration config, ISessionManager sessionManager, ILogger<TokenService> logger)
    {
        _config         = config;
        _sessionManager = sessionManager;
        _logger         = logger;
    }

    public async Task<AuthResponse> GenerateTokenAsync(string userId, string role, Guid? vendorId = null, bool isOwner = false, List<string> permissions = null)
    {
        var sessionId = Guid.NewGuid().ToString();
        var keyVal    = _config["Jwt:Key"] ?? "ThisIsASecretKeyForToyanaProjectAndItMustBeLongEnough";
        var key       = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyVal));
        var creds     = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
                     {
                         new(JwtRegisteredClaimNames.Sub, userId), // User ID
                         new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                         new("sessionId", sessionId),
                         new(ClaimTypes.Role, role)
                     };

        if (vendorId.HasValue)
        {
            claims.Add(new Claim("vendorId", vendorId.Value.ToString()));
            if (isOwner) claims.Add(new Claim("isOwner", "true"));
            if (permissions != null && permissions.Any())
                foreach (var p in permissions)
                    claims.Add(new Claim("permission", p));
        }

        var issuer   = _config["Jwt:Issuer"]   ?? "Toyana";
        var audience = _config["Jwt:Audience"] ?? "Toyana";

        var token = new JwtSecurityToken(
                                         issuer,
                                         audience,
                                         claims,
                                         expires: DateTime.UtcNow.AddMinutes(60),
                                         signingCredentials: creds
                                        );

        await _sessionManager.RegisterSessionAsync(Guid.Parse(userId), sessionId);

        _logger.LogDebug("Token generated for User {UserId} (Role: {Role}).", userId, role);
        return new AuthResponse(new JwtSecurityTokenHandler().WriteToken(token), sessionId, token.ValidTo);
    }
}
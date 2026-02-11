using GameVault.Core.Entities;
using System.Security.Claims;

namespace GameVault.Core.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    Guid? GetUserIdFromToken(string token);
}

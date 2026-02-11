using GameVault.Core.DTOs.Users;

namespace GameVault.Core.DTOs.Auth;

public class AuthResponse
{
    public required string Token { get; set; }
    public required string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public required UserDto User { get; set; }
}

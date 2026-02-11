using GameVault.Core.DTOs.Common;
using GameVault.Core.DTOs.Users;

namespace GameVault.Core.Interfaces;

public interface IUserService
{
    Task<PagedResult<UserDto>> GetUsersAsync(int pageNumber, int pageSize);
    Task<UserDto?> GetUserByIdAsync(Guid id);
    Task<UserDto?> UpdateUserAsync(Guid id, UpdateUserRequest request);
    Task<bool> ChangePasswordAsync(Guid id, ChangePasswordRequest request);
    Task<bool> DeactivateUserAsync(Guid id);
    Task<bool> ActivateUserAsync(Guid id);
}

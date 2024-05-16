using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTOs;

namespace Ecommerce.Service.src.Interfaces
{
    public interface IUserService : IBaseService<UserReadDto, UserCreateDto, UserUpdateDto, QueryOptions>
    {
        Task<bool> UpdatePasswordAsync(Guid userId, string newPassword);
        Task<UserReadDto> UpdateRoleAsync(Guid userId, UserRoleUpdateDto roleUpdateDto);
        Task<bool> ResetPasswordAsync(Guid userId, string newPassword);

    }
}
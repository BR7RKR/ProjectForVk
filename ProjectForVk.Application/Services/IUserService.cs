using System.Globalization;
using ProjectForVk.Core.Entities.DB;

namespace ProjectForVk.Application.Services;

public interface IUserService
{
    public Task AddUserAsync(UserEntity userEntity);
    public Task BlockUserAsync(string id);
    public Task<UserEntity> GetUserAsync(string id);
    public Task<IEnumerable<UserEntity>> GetUsersAsync(IEnumerable<string> ids);
}
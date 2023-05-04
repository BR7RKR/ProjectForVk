using ProjectForVk.Application.Services;
using ProjectForVk.Core.Entities.DB;

namespace ProjectForVk.Infrastructure.Services;

public sealed class UserService : IUserService
{
    public Task AddUserAsync(UserEntity userEntity)
    {
        throw new NotImplementedException();
    }

    public Task BlockUserAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<UserEntity> GetUserAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<UserEntity>> GetUsersAsync(IEnumerable<string> ids)
    {
        throw new NotImplementedException();
    }
}
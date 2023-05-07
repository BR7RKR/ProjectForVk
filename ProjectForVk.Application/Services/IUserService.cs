using ProjectForVk.Core.Entities.DB;
using ProjectForVk.Core.Entities.DTO;

namespace ProjectForVk.Application.Services;

public interface IUserService
{
    public Task AddUserAsync(UserDtoEntity userDto);
    public Task BlockUserAsync(int id);
    public Task<UserEntity> GetUserAsync(int id);
    public Task<IEnumerable<UserEntity>> GetUsersAsync(PaginationFilterDto filter);
    public Task<UserEntity> Authenticate(string login, string password);
}
using Microsoft.EntityFrameworkCore;
using ProjectForVk.Application.Services;
using ProjectForVk.Core.Entities.DB;
using ProjectForVk.Core.Entities.DTO;
using ProjectForVk.Core.Entities.Types;
using ProjectForVk.Infrastructure.Database;

namespace ProjectForVk.Infrastructure.Services;

internal sealed class UserService : IUserService
{
    private readonly ApplicationContext _context;

    public UserService(ApplicationContext context)
    {
        _context = context;
    }
    
    public async Task AddUserAsync(UserDtoEntity userDto)
    {

        var userWithSameId = await _context.Users.FindAsync(userDto.Id);

        if (userWithSameId is not null)
        {
            throw new Exception("User with the same id already exists");
        }

        var userEntity = await CreateUserEntityFromDto(userDto);

        await _context.Users.AddAsync(userEntity);
        await _context.SaveChangesAsync();
    }

    public async Task BlockUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user is null)
        {
            throw new Exception($"No user with guid :{id}");
        }

        user.UserState.Code = "Blocked";
        
        await _context.SaveChangesAsync();
    }

    public async Task<UserEntity> GetUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user is null)
        {
            throw new Exception($"No user with id :{id}");
        }

        return user;
    }

    public async Task<IEnumerable<UserEntity>> GetUsersAsync()
    {
        var users = await _context.Users.ToListAsync();

        if (users.Count == 0)
        {
            throw new Exception("No users in db");
        }
        
        return users;
    }
    
    private async Task<UserEntity> CreateUserEntityFromDto(UserDtoEntity userDto)
    {
        var userEntity = new UserEntity
        {
            Id = userDto.Id,
            Login = userDto.Login,
            Password = userDto.Password,
            CreatedDate = userDto.CreatedDate,
            UserGroupId = userDto.UserGroupId,
            UserStateId = userDto.UserStateId
        };
        var existingState = await _context.UserStates.FindAsync(userEntity.UserStateId);
        var existingGroup = await _context.UserGroups.FindAsync(userEntity.UserGroupId);

        userEntity.UserState = existingState;
        userEntity.UserGroup = existingGroup;
        return userEntity;
    }
}
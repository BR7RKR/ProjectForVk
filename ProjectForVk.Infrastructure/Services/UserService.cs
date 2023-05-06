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

    private const int TIME_TO_ADD = 5000;
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
        
        await Task.Delay(TIME_TO_ADD);

        await ActivateUserAsync(userEntity);
    }

    public async Task BlockUserAsync(int id)
    {
        var user = await _context.Users.Include(x => x.UserState).FirstOrDefaultAsync(x => x.Id == id);

        if (user is null)
        {
            throw new Exception($"No user with id :{id}");
        }

        if (user.UserState.Code == StateCodeType.Blocked)
        {
            throw new Exception("User is already blocked");
        }

        var blockedState = await _context.UserStates.FirstOrDefaultAsync(u => u.Code == StateCodeType.Blocked);

        if (blockedState is null)
        {
            throw new Exception($"No state with code 'Blocked'");
        }

        user.UserStateId = blockedState.Id;
        
        await _context.SaveChangesAsync();
    }

    public async Task<UserEntity> GetUserAsync(int id)
    {
        var user = await _context.Users.Include(x => x.UserGroup).Include(x => x.UserState).FirstOrDefaultAsync(x => x.Id == id);

        if (user is null)
        {
            throw new Exception($"No user with id :{id}");
        }

        return user;
    }

    public async Task<IEnumerable<UserEntity>> GetUsersAsync()
    {
        var users = await _context.Users.Include(x => x.UserGroup).Include(x => x.UserState).ToListAsync();

        if (users.Count == 0)
        {
            throw new Exception("No users in db");
        }
        
        return users;
    }
    
    private async Task ActivateUserAsync(UserEntity userEntity)
    {
        var activeState = await _context.UserStates.FirstOrDefaultAsync(s => s.Code == StateCodeType.Active);

        if (activeState is null)
        {
            throw new Exception("There is no state called 'Active'");
        }

        userEntity.UserStateId = activeState.Id;
        
        _context.Users.Update(userEntity);
        await _context.SaveChangesAsync();
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

        if (existingState is null)
        {
            throw new Exception($"No state with id: {userEntity.UserStateId}");
        }

        if (existingGroup is null)
        {
            throw new Exception($"No group with id: {userEntity.UserGroupId}");
        }
        
        if (existingGroup.Code == GroupCodeType.Admin)
        {
            await ValidateAdminCreationAsync();
        }
        
        return userEntity;
    }

    private async Task ValidateAdminCreationAsync()
    {
        var admin = await _context.Users.Include(u => u.UserGroup)
            .FirstOrDefaultAsync(u => u.UserGroup.Code == GroupCodeType.Admin);

        if (admin is not null)
        {
            throw new Exception("Admin already Exists");
        }
    }
}
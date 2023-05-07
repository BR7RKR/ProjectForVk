using Microsoft.EntityFrameworkCore;
using ProjectForVk.Application.Services;
using ProjectForVk.Core.Entities.DB;
using ProjectForVk.Core.Entities.DTO;
using ProjectForVk.Core.Entities.Types;
using ProjectForVk.Core.Exceptions.Group;
using ProjectForVk.Core.Exceptions.State;
using ProjectForVk.Core.Exceptions.User;
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
            throw new UserAlreadyExistsException(userDto.Id);
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
            throw new UserNotFoundException(id);
        }

        if (user.UserState.Code == StateCodeType.Blocked)
        {
            throw new UserBlockedException(id);
        }

        var blockedState = await _context.UserStates.FirstOrDefaultAsync(u => u.Code == StateCodeType.Blocked);

        if (blockedState is null)
        {
            throw new StateNotFoundException(StateCodeType.Blocked);
        }

        user.UserStateId = blockedState.Id;
        
        await _context.SaveChangesAsync();
    }

    public async Task<UserEntity> GetUserAsync(int id)
    {
        var user = await _context.Users
            .Include(x => x.UserGroup)
            .Include(x => x.UserState)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user is null)
        {
            throw new UserNotFoundException(id);
        }

        return user;
    }

    public async Task<IEnumerable<UserEntity>> GetUsersAsync(PaginationFilterDto filter)
    {
        var validFilter = new PaginationFilterDto(filter.PageNumber, filter.PageSize);
        var users = await _context.Users
            .Include(x => x.UserGroup)
            .Include(x => x.UserState)
            .Skip((validFilter.PageNumber-1) * validFilter.PageSize)
            .Take(validFilter.PageSize)
            .ToListAsync();
        
        if (users.Count == 0)
        {
            throw new UsersNotFoundException();
        }
        
        return users;
    }
    
    public async Task<UserEntity> Authenticate(string login, string password)
    {
        var user = await _context.Users
            .Include(x => x.UserGroup)
            .Include(x => x.UserState)
            .FirstOrDefaultAsync(u => u.Login == login && u.Password == password);

        if (user is null)
        {
            throw new UserNotFoundException(login);
        }

        if (user.UserState.Code == StateCodeType.Blocked)
        {
            throw new UserBlockedException(user.Id);
        }

        if (user.UserGroup.Code != GroupCodeType.Admin)
        {
            throw new NotEnoughPermissionsException(user.Id);
        }
        
        return user;
    }
    
    private async Task ActivateUserAsync(UserEntity userEntity)
    {
        var activeState = await _context.UserStates.FirstOrDefaultAsync(s => s.Code == StateCodeType.Active);

        if (activeState is null)
        {
            throw new StateNotFoundException(StateCodeType.Active);
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
            throw new StateNotFoundException(userEntity.UserStateId);
        }

        if (existingGroup is null)
        {
            throw new GroupNotFoundException(userEntity.UserGroupId);
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
            throw new AdminAlreadyExistsException(admin.Id);
        }
    }
}
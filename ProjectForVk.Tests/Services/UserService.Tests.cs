using Microsoft.EntityFrameworkCore;
using ProjectForVk.Core.Entities.DB;
using ProjectForVk.Core.Entities.DTO;
using ProjectForVk.Core.Entities.Types;
using ProjectForVk.Core.Exceptions.Group;
using ProjectForVk.Core.Exceptions.State;
using ProjectForVk.Core.Exceptions.User;
using ProjectForVk.Infrastructure.Database;
using ProjectForVk.Infrastructure.Services;

namespace ProjectForVk.Tests.Services;

public class UserServiceTests : DatabaseTestsHelper
{
    private readonly DbContextOptions<ApplicationContext> _contextOptions;

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task AddUserAsync_WithoutDuplicates_ShouldCreateUser(int groupId)
    {
        await using var context = CreateContext();
        await ClearDatabase(context);
        var service = CreateUserService(context);
        var userDto = DefaultUserDtoEntity(userGroupId: groupId);
        await SetUpGroups(context);
        await SetUpStates(context);

        await service.AddUserAsync(userDto);

        var userFromDb = await context.Users.FindAsync(userDto.Id);
        
        Assert.NotNull(userFromDb);
    }
    
    [Fact]
    public async Task AddUserAsync_WithDuplicates_ShouldThrowUserAlreadyExistsException()
    {
        await using var context = CreateContext();
        await ClearDatabase(context);
        var service = CreateUserService(context);
        var userDto = DefaultUserDtoEntity();
        var user = DefaultUserEntity();
        await SetUpGroups(context);
        await SetUpStates(context);
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        
        await Assert.ThrowsAsync<UserAlreadyExistsException>(() => service.AddUserAsync(userDto));
        
        var usersFromDb = await context.Users.ToListAsync();
        Assert.Single(usersFromDb);
    }
    
    [Fact]
    public async Task AddUserAsync_WithoutState_ShouldThrowStateNotFoundException()
    {
        await using var context = CreateContext();
        await ClearDatabase(context);
        var service = CreateUserService(context);
        var userDto = DefaultUserDtoEntity();
        await SetUpGroups(context);

        await Assert.ThrowsAsync<StateNotFoundException>(() => service.AddUserAsync(userDto));

        var usersFromDb = await context.Users.ToListAsync();
        Assert.True(usersFromDb.Count == 0);
    }
    
    [Fact]
    public async Task AddUserAsync_WithoutGroup_ShouldThrowGroupNotFoundException()
    {
        await using var context = CreateContext();
        await ClearDatabase(context);
        var service = CreateUserService(context);
        var userDto = DefaultUserDtoEntity();
        await SetUpStates(context);

        await Assert.ThrowsAsync<GroupNotFoundException>(() => service.AddUserAsync(userDto));

        var usersFromDb = await context.Users.ToListAsync();
        Assert.True(usersFromDb.Count == 0);
    }
    
    [Fact]
    public async Task AddUserAsync_WithAdmin_ShouldThrowAdminAlreadyExistsException()
    {
        var adminGroupId = 1;
        await using var context = CreateContext();
        await ClearDatabase(context);
        var service = CreateUserService(context);
        var userDto = DefaultUserDtoEntity(userGroupId: adminGroupId);
        var user = DefaultUserEntity(id: 1, userGroupId: adminGroupId);
        await SetUpStates(context);
        await SetUpGroups(context);
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<AdminAlreadyExistsException>(() => service.AddUserAsync(userDto));

        var usersFromDb = await context.Users.ToListAsync();
        Assert.Single(usersFromDb);
    }
    
    [Fact]
    public async Task BlockUserAsync_WithActiveUser_ShouldBlockUser()
    {
        var activeStateId = 1;
        var blockStateId = 0;
        await using var context = CreateContext();
        await ClearDatabase(context);
        var service = CreateUserService(context);
        await SetUpGroups(context);
        await SetUpStates(context);
        var user = DefaultUserEntity(userStateId: activeStateId);
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        await service.BlockUserAsync(user.Id);

        var userFromDb = await context.Users.FindAsync(user.Id);
        Assert.NotNull(userFromDb);
        Assert.Equal(blockStateId, userFromDb.UserStateId);
    }
    
    [Fact]
    public async Task BlockUserAsync_WithoutUser_ShouldThrowUserNotFoundException()
    {
        var blockStateId = 0;
        await using var context = CreateContext();
        await ClearDatabase(context);
        var service = CreateUserService(context);
        await SetUpGroups(context);
        await SetUpStates(context);
        var user = DefaultUserEntity(userStateId: blockStateId);

        await Assert.ThrowsAnyAsync<UserNotFoundException>(()=>service.BlockUserAsync(user.Id));
        
        var usersFromDb = await context.Users.ToListAsync();
        Assert.True(usersFromDb.Count == 0);
    }
    
    [Fact]
    public async Task BlockUserAsync_WithBlockedUser_ShouldThrowUserBlockedException()
    {
        var blockStateId = 0;
        await using var context = CreateContext();
        await ClearDatabase(context);
        var service = CreateUserService(context);
        await SetUpGroups(context);
        await SetUpStates(context);
        var user = DefaultUserEntity(userStateId: blockStateId);
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        await Assert.ThrowsAnyAsync<UserBlockedException>(()=>service.BlockUserAsync(user.Id));

        var userFromDb = await context.Users.FindAsync(user.Id);
        Assert.NotNull(userFromDb);
        Assert.Equal(blockStateId, userFromDb.UserStateId);
    }
    
    [Fact]
    public async Task BlockUserAsync_WithoutBlockState_ShouldThrowStateNotFoundException()
    {
        await using var context = CreateContext();
        await ClearDatabase(context);
        var service = CreateUserService(context);
        await SetUpGroups(context);
        var stateActive = DefaultStateEntity(1, StateCodeType.Active);
        await context.UserStates.AddAsync(stateActive);
        var user = DefaultUserEntity(userStateId: stateActive.Id);
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<StateNotFoundException>(()=>service.BlockUserAsync(user.Id));
    }

    [Fact]
    public async Task GetUserAsync_WithUser_ShouldReturnUser()
    {
        await using var context = CreateContext();
        await ClearDatabase(context);
        var service = CreateUserService(context);
        await SetUpGroups(context);
        await SetUpStates(context);
        var user = DefaultUserEntity();
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var userEntity = await service.GetUserAsync(user.Id);
        
        Assert.NotNull(userEntity);
        Assert.True(userEntity == user);
    }
    
    [Fact]
    public async Task GetUserAsync_WithoutUser_ShouldThrowUserNotFoundException()
    {
        await using var context = CreateContext();
        await ClearDatabase(context);
        var service = CreateUserService(context);
        await SetUpGroups(context);
        await SetUpStates(context);
        var user = DefaultUserEntity();

        await Assert.ThrowsAsync<UserNotFoundException>(() => service.GetUserAsync(user.Id));
        
        var usersFromDb = await context.Users.ToListAsync();
        Assert.True(usersFromDb.Count == 0);
    }
    
    [Fact]
    public async Task GetUsersAsync_WithUsers_ShouldReturnUsers()
    {
        await using var context = CreateContext();
        await ClearDatabase(context);
        var service = CreateUserService(context);
        await SetUpGroups(context);
        await SetUpStates(context);
        var filter = new PaginationFilterDto();
        var user1 = DefaultUserEntity();
        var user2 = DefaultUserEntity(1, login: "newLogin");
        await context.Users.AddAsync(user1);
        await context.Users.AddAsync(user2);
        await context.SaveChangesAsync();

        var users = await service.GetUsersAsync(filter);
        
        Assert.NotNull(users);
        Assert.True(users.ToArray().Length == 2);
    }
    
    [Fact]
    public async Task GetUsersAsync_WithoutUsers_ShouldThrowUsersNotFoundException()
    {
        await using var context = CreateContext();
        await ClearDatabase(context);
        var service = CreateUserService(context);
        await SetUpGroups(context);
        await SetUpStates(context);
        var filter = new PaginationFilterDto();
        var user1 = DefaultUserEntity();
        var user2 = DefaultUserEntity(1);

        await Assert.ThrowsAsync<UsersNotFoundException>(() => service.GetUsersAsync(filter));
        
        var usersFromDb = await context.Users.ToListAsync();
        Assert.True(usersFromDb.Count == 0);
    }

    private UserDtoEntity DefaultUserDtoEntity(int id = 0, string login = "string", string password = "string", DateOnly createdDate = default, int userGroupId = 0, int userStateId = 0)
    {
        var user = new UserDtoEntity
        {
            Id = id,
            Login = login,
            Password = password,
            CreatedDate = createdDate,
            UserGroupId = userGroupId,
            UserStateId = userStateId
        };

        return user;
    }
    
    private UserEntity DefaultUserEntity(int id = 0, string login = "string", string password = "string", DateOnly createdDate = default, int userGroupId = 0, int userStateId = 0)
    {
        var user = new UserEntity
        {
            Id = id,
            Login = login,
            Password = password,
            CreatedDate = createdDate,
            UserGroupId = userGroupId,
            UserStateId = userStateId
        };

        return user;
    }

    private async Task SetUpGroups(ApplicationContext context)
    {
        var groupUser = DefaultGroupEntity();
        var groupAdmin = DefaultGroupEntity(1, GroupCodeType.Admin);

        await context.UserGroups.AddAsync(groupUser);
        await context.UserGroups.AddAsync(groupAdmin);
        await context.SaveChangesAsync();
    }

    private async Task SetUpStates(ApplicationContext context)
    {
        var stateBlocked = DefaultStateEntity();
        var stateActive = DefaultStateEntity(1, StateCodeType.Active);
        await context.UserStates.AddAsync(stateBlocked);
        await context.UserStates.AddAsync(stateActive);
        await context.SaveChangesAsync();
    }
    
    private UserGroupEntity DefaultGroupEntity(int id = 0, GroupCodeType code = GroupCodeType.User)
    {
        var group = new UserGroupEntity
        {
            Id = id,
            Code = code,
            Description = "empty"
        };

        return group;
    }
    
    private UserStateEntity DefaultStateEntity(int id = 0, StateCodeType code = StateCodeType.Blocked)
    {
        var state = new UserStateEntity
        {
            Id = id,
            Code = code,
            Description = "empty"
        };

        return state;
    }
    
    
    private UserService CreateUserService(ApplicationContext context)
    {
        return new UserService(context);
    }
}
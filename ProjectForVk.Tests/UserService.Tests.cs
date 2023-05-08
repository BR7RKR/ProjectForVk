using Microsoft.EntityFrameworkCore;
using ProjectForVk.Core.Entities.DB;
using ProjectForVk.Core.Entities.DTO;
using ProjectForVk.Core.Entities.Types;
using ProjectForVk.Core.Exceptions.Group;
using ProjectForVk.Core.Exceptions.State;
using ProjectForVk.Core.Exceptions.User;
using ProjectForVk.Infrastructure.Database;
using ProjectForVk.Infrastructure.Services;

namespace ProjectForVk.Tests;

public class UserServiceTests
{
    private readonly DbContextOptions<ApplicationContext> _contextOptions;

    public UserServiceTests()
    {
        _contextOptions = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }
    
    private ApplicationContext CreateContext()
    {
        return new ApplicationContext(_contextOptions);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task AddUserAsync_WithoutDuplicates_ShouldCreateUser(int groupId)
    {
        var context = CreateContext();
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
        var context = CreateContext();
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
        var context = CreateContext();
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
        var context = CreateContext();
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
        var context = CreateContext();
        var service = CreateUserService(context);
        var userDto = DefaultUserDtoEntity(userGroupId: adminGroupId);
        var user = DefaultUserEntity(id: 1, userGroupId: adminGroupId);
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        await SetUpStates(context);
        await SetUpGroups(context);

        await Assert.ThrowsAsync<AdminAlreadyExistsException>(() => service.AddUserAsync(userDto));

        var usersFromDb = await context.Users.ToListAsync();
        Assert.Single(usersFromDb);
    }
    
    [Fact]
    public async Task BlockUserAsync_WithActiveUser_ShouldBlockUser()
    {
        var activeStateId = 1;
        var blockStateId = 0;
        var context = CreateContext();
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
        var context = CreateContext();
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
        var context = CreateContext();
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
        var context = CreateContext();
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
    public async Task AuthenticateAsync_WithProperLoginData_ShouldAuthorizeUser()
    {
        var adminGroupId = 1;
        var activeStateId = 1;
        var context = CreateContext();
        var service = CreateUserService(context);
        await SetUpGroups(context);
        await SetUpStates(context);
        var admin = DefaultUserEntity(userStateId: activeStateId, userGroupId: adminGroupId);
        await context.Users.AddAsync(admin);
        await context.SaveChangesAsync();

        var userEntity = await service.Authenticate(admin.Login, admin.Password);
        
        Assert.True(admin == userEntity);
    }
    
    [Fact]
    public async Task AuthenticateAsync_WithoutProperLogin_ShouldThrowUserNotFoundException()
    {
        var adminGroupId = 1;
        var activeStateId = 1;
        var context = CreateContext();
        var service = CreateUserService(context);
        await SetUpGroups(context);
        await SetUpStates(context);
        var admin = DefaultUserEntity(userStateId: activeStateId, userGroupId: adminGroupId);
        await context.Users.AddAsync(admin);
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<UserNotFoundException>(() => service.Authenticate("", admin.Password));
        
        var usersFromDb = await context.Users.ToListAsync();
        Assert.True(usersFromDb.Count == 1);
    }
    
    [Fact]
    public async Task AuthenticateAsync_WithoutProperPassword_ShouldThrowUserNotFoundException()
    {
        var adminGroupId = 1;
        var activeStateId = 1;
        var context = CreateContext();
        var service = CreateUserService(context);
        await SetUpGroups(context);
        await SetUpStates(context);
        var admin = DefaultUserEntity(userStateId: activeStateId, userGroupId: adminGroupId);
        await context.Users.AddAsync(admin);
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<UserNotFoundException>(() => service.Authenticate(admin.Login, ""));
        
        var usersFromDb = await context.Users.ToListAsync();
        Assert.True(usersFromDb.Count == 1);
    }
    
    [Fact]
    public async Task AuthenticateAsync_WithoutUser_ShouldThrowUserNotFoundException()
    {
        var adminGroupId = 1;
        var activeStateId = 1;
        var context = CreateContext();
        var service = CreateUserService(context);
        await SetUpGroups(context);
        await SetUpStates(context);
        var admin = DefaultUserEntity(userStateId: activeStateId, userGroupId: adminGroupId);

        await Assert.ThrowsAsync<UserNotFoundException>(() => service.Authenticate(admin.Login, admin.Password));
        
        var usersFromDb = await context.Users.ToListAsync();
        Assert.True(usersFromDb.Count == 0);
    }
    
    [Fact]
    public async Task AuthenticateAsync_WithBlockedUser_ShouldThrowUserBlockedException()
    {
        var adminGroupId = 1;
        var blockStateId = 0;
        var context = CreateContext();
        var service = CreateUserService(context);
        await SetUpGroups(context);
        await SetUpStates(context);
        var admin = DefaultUserEntity(userStateId: blockStateId, userGroupId: adminGroupId);
        await context.Users.AddAsync(admin);
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<UserBlockedException>(() => service.Authenticate(admin.Login, admin.Password));
        
        var adminFromDb = await context.Users.FindAsync(admin.Id);
        Assert.NotNull(adminFromDb);
        Assert.Equal(blockStateId, adminFromDb.UserStateId);
    }
    
    [Fact]
    public async Task AuthenticateAsync_WithoutProperPermissions_ShouldThrowNotEnoughPermissionsException()
    {
        var userGroupId = 0;
        var activeStateId = 1;
        var context = CreateContext();
        var service = CreateUserService(context);
        await SetUpGroups(context);
        await SetUpStates(context);
        var admin = DefaultUserEntity(userStateId: activeStateId, userGroupId: userGroupId);
        await context.Users.AddAsync(admin);
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<NotEnoughPermissionsException>(() => service.Authenticate(admin.Login, admin.Password));
        
        var adminFromDb = await context.Users.FindAsync(admin.Id);
        Assert.NotNull(adminFromDb);
        Assert.Equal(userGroupId, adminFromDb.UserGroupId);
    }
    
    [Fact]
    public async Task GetUserAsync_WithUser_ShouldReturnUser()
    {
        var context = CreateContext();
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
        var context = CreateContext();
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
        var context = CreateContext();
        var service = CreateUserService(context);
        await SetUpGroups(context);
        await SetUpStates(context);
        var filter = new PaginationFilterDto();
        var user1 = DefaultUserEntity();
        var user2 = DefaultUserEntity(1);
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
        var context = CreateContext();
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
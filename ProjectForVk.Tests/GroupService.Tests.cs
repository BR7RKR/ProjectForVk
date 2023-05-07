using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProjectForVk.Core.Entities.DB;
using ProjectForVk.Core.Entities.Types;
using ProjectForVk.Infrastructure.Database;
using ProjectForVk.Infrastructure.Services;

namespace ProjectForVk.Tests;

public sealed class GroupServiceTests // Не совсем понимаю зачем юнит тесты в приложении с таким функционалом.
{                                     // Как по мне больше смысла бы имели интеграционные тесты.
    private readonly DbContextOptions<ApplicationContext> _contextOptions;

    public GroupServiceTests()
    {
        _contextOptions = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase("test")
            .Options;
    }

    [Theory]
    [InlineData(0, GroupCodeType.User)]
    [InlineData(1, GroupCodeType.Admin)]
    public async Task CreateGroupAsync_WithoutDuplicates_ShouldCreateGroup(int id,GroupCodeType codeType)
    {
        var context = CreateContext();
        var groupService = CreateGroupService(context);
        var group = DefaultGroupEntity(id, codeType);

        await groupService.AddGroupAsync(group);

        var groupFromDb = await context.UserGroups.FindAsync(group.Id);
        
        Assert.NotNull(groupFromDb);
        Assert.True(group == groupFromDb);
    }
    
    private ApplicationContext CreateContext()
    {
        return new ApplicationContext(_contextOptions);
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
    
    private GroupService CreateGroupService(ApplicationContext context)
    {
        return new GroupService(context);
    }
}
using Microsoft.EntityFrameworkCore;
using ProjectForVk.Core.Entities.DB;
using ProjectForVk.Core.Entities.Types;
using ProjectForVk.Core.Exceptions.Group;
using ProjectForVk.Infrastructure.Database;
using ProjectForVk.Infrastructure.Services;

namespace ProjectForVk.Tests.Services;

public sealed class GroupServiceTests : DatabaseTestsHelper
{
    [Theory]
    [InlineData(GroupCodeType.User)]
    [InlineData(GroupCodeType.Admin)]
    public async Task CreateGroupAsync_WithoutDuplicates_ShouldCreateGroup(GroupCodeType codeType)
    {
        await using var context = CreateContext();
        await ClearDatabase(context);
        var service = CreateGroupService(context);
        var group = DefaultGroupEntity(code: codeType);
        await service.AddGroupAsync(group);

        var groupFromDb = await context.UserGroups.FindAsync(group.Id);
        
        Assert.NotNull(groupFromDb);
        Assert.True(group == groupFromDb);
    }
    
    [Fact]
    public async Task CreateGroupAsync_WithDuplicates_ShouldThrowGroupAlreadyExistsException()
    {
        await using var context = CreateContext();
        await ClearDatabase(context);
        var service = CreateGroupService(context);
        var group = DefaultGroupEntity();
        var groupDuplicate = DefaultGroupEntity();
        await context.UserGroups.AddAsync(group);
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<GroupAlreadyExistsException>(() => service.AddGroupAsync(groupDuplicate));
        
        var groupsFromDb = await context.UserGroups.ToListAsync();
        Assert.Single(groupsFromDb);
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
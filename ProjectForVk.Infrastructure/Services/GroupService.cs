using ProjectForVk.Application.Services;
using ProjectForVk.Core.Entities.DB;
using ProjectForVk.Core.Exceptions.Group;
using ProjectForVk.Infrastructure.Database;

namespace ProjectForVk.Infrastructure.Services;

public sealed class GroupService : IGroupService
{
    private readonly ApplicationContext _context;

    public GroupService(ApplicationContext context)
    {
        _context = context;
    }
    
    public async Task AddGroupAsync(UserGroupEntity groupEntity)
    {
        var groupWithSameId = await _context.UserGroups.FindAsync(groupEntity.Id);

        if (groupWithSameId is not null)
        {
            throw new GroupAlreadyExistsException(groupEntity.Id);
        }

        await _context.UserGroups.AddAsync(groupEntity);
        await _context.SaveChangesAsync();
    }
}
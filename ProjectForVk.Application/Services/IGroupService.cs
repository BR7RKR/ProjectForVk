using ProjectForVk.Core.Entities.DB;

namespace ProjectForVk.Application.Services;

public interface IGroupService
{
    public Task AddGroupAsync(UserGroupEntity groupEntity);
}
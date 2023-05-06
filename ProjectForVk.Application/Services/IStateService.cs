using ProjectForVk.Core.Entities.DB;

namespace ProjectForVk.Application.Services;

public interface IStateService
{
    public Task AddStateAsync(UserStateEntity stateEntity);
}
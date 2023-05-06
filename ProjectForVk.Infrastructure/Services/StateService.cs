using ProjectForVk.Application.Services;
using ProjectForVk.Core.Entities.DB;
using ProjectForVk.Core.Exceptions.State;
using ProjectForVk.Infrastructure.Database;

namespace ProjectForVk.Infrastructure.Services;

internal sealed class StateService : IStateService
{
    private readonly ApplicationContext _context;

    public StateService(ApplicationContext context)
    {
        _context = context;
    }
    
    public async Task AddStateAsync(UserStateEntity stateEntity)
    {
        var groupWithSameId = await _context.UserStates.FindAsync(stateEntity.Id);

        if (groupWithSameId is not null)
        {
            throw new StateAlreadyExistsException(stateEntity.Id);
        }

        await _context.UserStates.AddAsync(stateEntity);
        await _context.SaveChangesAsync();
    }
}
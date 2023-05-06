using ProjectForVk.Application.Services;
using ProjectForVk.Core.Entities.DB;
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
            throw new Exception("State with the same id already exists");
        }

        await _context.UserStates.AddAsync(stateEntity);
        await _context.SaveChangesAsync();
    }
}
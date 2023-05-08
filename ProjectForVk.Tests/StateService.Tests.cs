using Microsoft.EntityFrameworkCore;
using ProjectForVk.Core.Entities.DB;
using ProjectForVk.Core.Entities.Types;
using ProjectForVk.Core.Exceptions.State;
using ProjectForVk.Infrastructure.Database;
using ProjectForVk.Infrastructure.Services;

namespace ProjectForVk.Tests;

public class StateServiceTests
{
    private readonly DbContextOptions<ApplicationContext> _contextOptions;

    public StateServiceTests()
    {
        _contextOptions = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    [Theory]
    [InlineData(StateCodeType.Active)]
    [InlineData(StateCodeType.Blocked)]
    public async Task CreateStateAsync_WithoutDuplicates_ShouldCreateState(StateCodeType codeType)
    {
        var context = CreateContext();
        var service = CreateStateService(context);
        var state = DefaultStateEntity(code: codeType);

        await service.AddStateAsync(state);

        var stateFromDb = await context.UserStates.FindAsync(state.Id);
        
        Assert.NotNull(stateFromDb);
        Assert.True(state == stateFromDb);
    }
    
    [Fact]
    public async Task CreateStateAsync_WithDuplicates_ShouldThrowStateAlreadyExistsException()
    {
        var context = CreateContext();
        var service = CreateStateService(context);
        var state = DefaultStateEntity();
        var stateDuplicate = DefaultStateEntity();
        await context.UserStates.AddAsync(state);
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<StateAlreadyExistsException>(() => service.AddStateAsync(stateDuplicate));
        
        var statesFromDb = await context.UserStates.ToListAsync();
        Assert.Single(statesFromDb);
    }
    
    private ApplicationContext CreateContext()
    {
        return new ApplicationContext(_contextOptions);
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
    
    private StateService CreateStateService(ApplicationContext context)
    {
        return new StateService(context);
    }
}
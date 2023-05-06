using ProjectForVk.Core.Entities.Types;

namespace ProjectForVk.Core.Exceptions.State;

public sealed class StateNotFoundException : Exception
{
    public StateNotFoundException(StateCodeType type) : base($"There is no state of type '{type}'"){}
    public StateNotFoundException(int id) : base($"There is no state with id = '{id}'"){}
}
namespace ProjectForVk.Core.Exceptions.State;

public sealed class StateAlreadyExistsException : Exception
{
    public StateAlreadyExistsException(int id) : base($"State with id = '{id}' already exists"){}
}
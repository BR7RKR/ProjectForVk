namespace ProjectForVk.Core.Exceptions.Group;

public sealed class GroupNotFoundException : Exception
{
    public GroupNotFoundException(int id) : base($"There is no group with id = '{id}'"){}
}
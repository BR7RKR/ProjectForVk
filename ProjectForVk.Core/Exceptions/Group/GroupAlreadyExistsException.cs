namespace ProjectForVk.Core.Exceptions.Group;

public sealed class GroupAlreadyExistsException : Exception
{
    public GroupAlreadyExistsException(int id) : base($"Group with id = '{id}' already exists"){}
}
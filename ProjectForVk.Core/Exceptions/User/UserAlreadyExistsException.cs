namespace ProjectForVk.Core.Exceptions.User;

public sealed class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException(int id) : base($"User with id = '{id}' already exists"){}
}
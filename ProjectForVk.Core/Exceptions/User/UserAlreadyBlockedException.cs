namespace ProjectForVk.Core.Exceptions.User;

public sealed class UserAlreadyBlockedException : Exception
{
    public UserAlreadyBlockedException(int id) : base($"User with id = '{id}' is already blocked"){}
}
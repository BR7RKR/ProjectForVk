namespace ProjectForVk.Core.Exceptions.User;

public sealed class UserBlockedException : Exception
{
    public UserBlockedException(int id) : base($"User with id = '{id}' is blocked"){}
}
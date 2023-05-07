namespace ProjectForVk.Core.Exceptions.User;

public sealed class NotEnoughPermissionsException : Exception
{
    public NotEnoughPermissionsException(int id) : base($"User with id = '{id}' doesn't have enough permissions"){}
}
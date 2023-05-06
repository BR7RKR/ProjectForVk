namespace ProjectForVk.Core.Exceptions.User;

public sealed class AdminAlreadyExistsException : Exception
{
    public AdminAlreadyExistsException(int id) : base($"Admin already exists with id = '{id}'"){}
}
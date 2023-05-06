namespace ProjectForVk.Core.Exceptions.User;

public sealed class UsersNotFoundException : Exception
{
    public UsersNotFoundException() : base("No users in database"){}
}
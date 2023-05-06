﻿namespace ProjectForVk.Core.Exceptions.User;

public sealed class UserNotFoundException : Exception
{
    public UserNotFoundException(int id) : base($"No user with id = {id}"){}
}
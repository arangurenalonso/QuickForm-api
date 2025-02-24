﻿namespace QuickForm.Common.Domain;
public interface IPasswordHashingService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}

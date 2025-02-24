﻿namespace QuickForm.Modules.Users.Application;
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

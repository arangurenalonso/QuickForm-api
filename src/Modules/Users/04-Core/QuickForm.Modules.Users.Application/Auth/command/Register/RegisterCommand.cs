﻿using QuickForm.Common.Application;

namespace QuickForm.Modules.Users.Application;
public sealed record RegisterCommand(
        string Email,
        string Password,
        string ConfirmPassword,
        string FirstName,
        string? LastName) : ICommand<ResultResponse>;

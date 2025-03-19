﻿using Microsoft.AspNetCore.Authorization;

namespace QuickForm.Common.Infrastructure;
internal sealed class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }

    public string Permission { get; }
}

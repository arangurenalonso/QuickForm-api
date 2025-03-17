﻿using System.ComponentModel;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public enum RoleType
{
    [Detail("Admin")]
    [Description("7C764050-2978-40E9-8D52-7B900FA6ABAC")]
    Admin,

    [Detail("Guest")]
    [Description("11CA0682-EB42-44E9-861B-A1D19F62072B")]
    Guest,
}

﻿using Microsoft.AspNetCore.Authorization;

namespace AutoGenerator.CustomPolicy;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string ClaimType { get; }
    public string ClaimValue { get; }

    public PermissionRequirement(string claimType, string claimValue)
    {
        ClaimType = claimType;
        ClaimValue = claimValue;
    }
}

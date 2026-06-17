// <copyright file="AuthContext.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Endpoint.Tests.Contexts;

using System.Diagnostics.CodeAnalysis;
using Defra.Identity.Endpoint.Tests.Configuration;

[ExcludeFromCodeCoverage]
public class AuthContext(
    ApiConfiguration configuration)
    : ITestContext
{
    public UserType UserType { get; set; }

    public Guid UserId { get; set; }

    public void SetupUser(UserType userType)
    {
        UserType = userType;
        switch (userType)
        {
            case UserType.Defra:
                UserId = configuration.DefraUserId;
                break;
            case UserType.Owner:
                UserId = configuration.OwnerUserId;
                break;
            case UserType.Keeper:
                UserId = configuration.KeeperUserId;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(userType), userType, null);
        }
    }
}

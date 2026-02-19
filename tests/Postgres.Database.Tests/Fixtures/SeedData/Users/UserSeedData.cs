// <copyright file="UserSeedData.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Fixtures.SeedData.Users;

using Defra.Identity.Postgres.Database.Entities;

public static class UserSeedData
{
    public const string AdminEmailAddress = "test@test.com";

    public static UserAccounts GetAdminUserEntity()
    {
        var id = Guid.NewGuid();

        return new UserAccounts()
        {
            Id = id,
            DisplayName = "Test User",
            EmailAddress = AdminEmailAddress,
            FirstName = "test",
            LastName = "user",
            CreatedById = id,
        };
    }
}

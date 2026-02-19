// <copyright file="SeedDataQueryHelper.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Fixtures.SeedData;

using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures.SeedData.Users;
using Microsoft.EntityFrameworkCore;

public static class SeedDataQueryHelper
{
    public static async Task<UserAccounts> GetAdminUser(PostgresDbContext context)
    {
        var adminUser = await context.UserAccounts
            .FirstOrDefaultAsync(user => user.EmailAddress == UserSeedData.AdminEmailAddress);

        return adminUser ?? throw new InvalidOperationException($"User {UserSeedData.AdminEmailAddress} not found");
    }
}

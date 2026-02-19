// <copyright file="TestDataHelper.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Fixtures.TestData.Helpers;

using Defra.Identity.Postgres.Database.Entities;
using Microsoft.EntityFrameworkCore;

public static class TestDataHelper
{
    public const string AdminEmailAddress = "test@test.com";

    public static async Task<UserAccounts> GetAdminUser(PostgresDbContext context)
    {
        var adminUser = await context.UserAccounts
            .FirstOrDefaultAsync(user => user.EmailAddress == AdminEmailAddress);

        return adminUser ?? throw new InvalidOperationException($"User {AdminEmailAddress} not found");
    }
}

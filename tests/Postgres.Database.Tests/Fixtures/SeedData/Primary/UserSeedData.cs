// <copyright file="UserSeedData.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Fixtures.SeedData.Primary;

using Defra.Identity.Postgres.Database.Entities;

public static class UserSeedData
{
    public const string AdminEmailAddress = "test@test.com";

    public static UserAccounts GetAdminUserEntity()
    {
        return new UserAccounts()
        {
            Id = new Guid("0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1"),
            DisplayName = "Test User",
            EmailAddress = AdminEmailAddress,
            FirstName = "test",
            LastName = "user",
            CreatedById = new Guid("0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1"),
        };
    }

    public static UserAccounts[] GetStandardUserEntities()
        =>
        [
            new()
            {
                Id = new Guid("42bde7a0-9efe-402a-a7c3-9161be7b00ba"),
                DisplayName = "Test User 1",
                EmailAddress = "test1@test.com",
                FirstName = "test 1",
                LastName = "user 1",
                CreatedAt = DateTime.Parse("2026-03-01").ToUniversalTime(),
                CreatedById = new Guid("0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1"),
            },
            new()
            {
                Id = new Guid("1e21b685-2247-4d96-bf39-f7dc30f356c2"),
                DisplayName = "Test User 2",
                EmailAddress = "test2@test.com",
                FirstName = "test 2",
                LastName = "user 2",
                CreatedAt = DateTime.Parse("2026-03-02").ToUniversalTime(),
                CreatedById = new Guid("0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1"),
            },
            new()
            {
                Id = new Guid("83bf35f9-fd59-4c8a-b70a-7d95a1aab2b6"),
                DisplayName = "Test User 3",
                EmailAddress = "test3@test.com",
                FirstName = "test 3",
                LastName = "user 3",
                CreatedAt = DateTime.Parse("2026-03-03").ToUniversalTime(),
                CreatedById = new Guid("0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1"),
            },
            new()
            {
                Id = new Guid("d1354eb1-dd1c-471e-bd0e-2626e2e21366"),
                DisplayName = "Test User 4",
                EmailAddress = "test4@test.com",
                FirstName = "test 4",
                LastName = "user 4",
                CreatedAt = DateTime.Parse("2026-03-04").ToUniversalTime(),
                CreatedById = new Guid("0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1"),
            },
            new()
            {
                Id = new Guid("2f778d40-965c-4479-b94d-86f66d100952"),
                DisplayName = "Test User 5",
                EmailAddress = "test5@test.com",
                FirstName = "test 5",
                LastName = "user 5",
                CreatedAt = DateTime.Parse("2026-03-05").ToUniversalTime(),
                CreatedById = new Guid("0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1"),
            },
            new()
            {
                Id = new Guid("dd1b98c5-0874-4718-a37c-74cdec359567"),
                DisplayName = "Test User 6",
                EmailAddress = "test6@test.com",
                FirstName = "test 6",
                LastName = "user 6",
                CreatedAt = DateTime.Parse("2026-03-06").ToUniversalTime(),
                CreatedById = new Guid("0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1"),
            },
            new()
            {
                Id = new Guid("2c6d9ca2-6e78-4fdd-88eb-61fa6cc7f319"),
                DisplayName = "Test User 7",
                EmailAddress = "test7@test.com",
                FirstName = "test 7",
                LastName = "user 7",
                CreatedAt = DateTime.Parse("2026-03-07").ToUniversalTime(),
                CreatedById = new Guid("0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1"),
            },
            new()
            {
                Id = new Guid("054037a5-e666-4d2f-9d4d-aa5efce35d7b"),
                DisplayName = "Test User 8",
                EmailAddress = "test8@test.com",
                FirstName = "test 8",
                LastName = "user 8",
                CreatedAt = DateTime.Parse("2026-03-08").ToUniversalTime(),
                CreatedById = new Guid("0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1"),
            },
            new()
            {
                Id = new Guid("7e9e5585-7b59-4d10-b330-1c95d83a4670"),
                DisplayName = "Test User 9",
                EmailAddress = "test9@test.com",
                FirstName = "test 9",
                LastName = "user 9",
                CreatedAt = DateTime.Parse("2026-03-09").ToUniversalTime(),
                CreatedById = new Guid("0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1"),
            },
            new()
            {
                Id = new Guid("4852c883-6dc2-4880-aaa8-c20110955c90"),
                DisplayName = "Test User 10",
                EmailAddress = "test10@test.com",
                FirstName = "test 10",
                LastName = "user 10",
                CreatedAt = DateTime.Parse("2026-03-10").ToUniversalTime(),
                CreatedById = new Guid("0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1"),
            },
            new()
            {
                Id = new Guid("6d0d343b-cbba-4cb7-bd3f-d7a9407f248d"),
                DisplayName = "Test User 11",
                EmailAddress = "test11@test.com",
                FirstName = "test 11",
                LastName = "user 11",
                CreatedAt = DateTime.Parse("2026-03-11").ToUniversalTime(),
                CreatedById = new Guid("0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1"),
            },
            new()
            {
                Id = new Guid("74bbb0da-5a57-48f9-abcb-7bedbfe87ede"),
                DisplayName = "Test User 12",
                EmailAddress = "test12@test.com",
                FirstName = "test 12",
                LastName = "user 12",
                CreatedAt = DateTime.Parse("2026-03-12").ToUniversalTime(),
                CreatedById = new Guid("0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1"),
            },
        ];
}

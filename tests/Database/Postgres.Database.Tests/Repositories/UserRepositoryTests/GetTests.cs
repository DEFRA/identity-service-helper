// <copyright file="GetTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.UserRepositoryTests;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Users;
using Defra.Identity.Test.Utilities.Assertions;
using Microsoft.Extensions.Logging;

public class GetTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Should get a single user")]
    public async Task ShouldGetSingleUser()
    {
        // Arrange
        var logger = DefraLoggerExtensions.CreateNSubstituteLogger<UserRepository>();
        var repository = new UserRepository(Context, ReadOnlyContext, logger);

        var adminUser = Context.UserAccounts.First();

        var userToQuery = new UserAccounts()
        {
            DisplayName = "Single Test User 1 Display Name",
            FirstName = "Single Test User 1 First Name",
            LastName = "Single Test User 1 Last Name",
            EmailAddress = "singletestuser1@test.com",
            KrdsId = new Guid("11cc4506-1ece-4970-9eac-93a26d7d4b80"),
            SamId = "SamId1",
            CreatedById = adminUser.Id,
            CreatedAt = DateTime.UtcNow.AddDays(2),
            DeletedById = adminUser.Id,
            DeletedAt = DateTime.UtcNow.AddDays(3),
        };

        await Context.UserAccounts.AddAsync(userToQuery, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await repository.GetSingle(
            user => user.DisplayName == "Single Test User 1 Display Name",
            TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();

        result.ShouldSatisfyAllConditions(
            x => x.DisplayName.ShouldBe("Single Test User 1 Display Name"),
            x => x.FirstName.ShouldBe("Single Test User 1 First Name"),
            x => x.LastName.ShouldBe("Single Test User 1 Last Name"),
            x => x.EmailAddress.ShouldBe("singletestuser1@test.com"),
            x => x.KrdsId.ShouldBe(new Guid("11cc4506-1ece-4970-9eac-93a26d7d4b80")),
            x => x.SamId.ShouldBe("SamId1"),
            x => x.CreatedById.ShouldBe(adminUser.Id),
            x => x.CreatedAt.ShouldBeCloseToUtcNowAddDays(2),
            x => x.DeletedById.ShouldBe(adminUser.Id),
            x => x.DeletedAt!.Value.ShouldBeCloseToUtcNowAddDays(3));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Getting single user account");
    }

    [Fact]
    [Description("Should get a list of users")]
    public async Task ShouldGetListUsers()
    {
        // Arrange
        var logger = DefraLoggerExtensions.CreateNSubstituteLogger<UserRepository>();
        var repository = new UserRepository(Context, ReadOnlyContext, logger);

        var adminUser = Context.UserAccounts.First();

        var users = new List<UserAccounts>
        {
            new()
            {
                DisplayName = "List Test User 1 Display Name",
                FirstName = "List Test User 1 First Name",
                LastName = "List Test User 1 Last Name",
                EmailAddress = "listtestuser1@test.com",
                KrdsId = new Guid("3a312595-21c8-46d7-8226-da3d01ec5610"),
                SamId = "SamId1",
                CreatedById = adminUser.Id,
                CreatedAt = DateTime.UtcNow.AddDays(2),
                DeletedById = adminUser.Id,
                DeletedAt = DateTime.UtcNow.AddDays(3),
            },
            new()
            {
                DisplayName = "List Test User 2 Display Name",
                FirstName = "List Test User 2 First Name",
                LastName = "List Test User 2 Last Name",
                EmailAddress = "listtestuser2@test.com",
                KrdsId = new Guid("219c1ddd-d498-4e54-b6b2-ebfd832ba899"),
                SamId = "SamId2",
                CreatedById = adminUser.Id,
                CreatedAt = DateTime.UtcNow.AddDays(2),
                DeletedById = adminUser.Id,
                DeletedAt = DateTime.UtcNow.AddDays(4),
            },
        };

        await Context.UserAccounts.AddRangeAsync(users, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var results =
            (await repository.GetList(
                x => x.DisplayName.Contains("List Test User"),
                TestContext.Current.CancellationToken))
            .OrderBy(x => x.DisplayName)
            .ToList();

        // Assert
        results.ShouldNotBeNull();
        results.Count.ShouldBe(2);

        results[0].ShouldSatisfyAllConditions(
            x => x.DisplayName.ShouldBe("List Test User 1 Display Name"),
            x => x.FirstName.ShouldBe("List Test User 1 First Name"),
            x => x.LastName.ShouldBe("List Test User 1 Last Name"),
            x => x.EmailAddress.ShouldBe("listtestuser1@test.com"),
            x => x.KrdsId.ShouldBe(new Guid("3a312595-21c8-46d7-8226-da3d01ec5610")),
            x => x.SamId.ShouldBe("SamId1"),
            x => x.CreatedById.ShouldBe(adminUser.Id),
            x => x.CreatedAt.ShouldBeCloseToUtcNowAddDays(2),
            x => x.DeletedById.ShouldBe(adminUser.Id),
            x => x.DeletedAt!.Value.ShouldBeCloseToUtcNowAddDays(3));

        results[1].ShouldSatisfyAllConditions(
            x => x.DisplayName.ShouldBe("List Test User 2 Display Name"),
            x => x.FirstName.ShouldBe("List Test User 2 First Name"),
            x => x.LastName.ShouldBe("List Test User 2 Last Name"),
            x => x.EmailAddress.ShouldBe("listtestuser2@test.com"),
            x => x.KrdsId.ShouldBe(new Guid("219c1ddd-d498-4e54-b6b2-ebfd832ba899")),
            x => x.SamId.ShouldBe("SamId2"),
            x => x.CreatedById.ShouldBe(adminUser.Id),
            x => x.CreatedAt.ShouldBeCloseToUtcNowAddDays(2),
            x => x.DeletedById.ShouldBe(adminUser.Id),
            x => x.DeletedAt!.Value.ShouldBeCloseToUtcNowAddDays(4));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Getting list of user accounts");
    }
}

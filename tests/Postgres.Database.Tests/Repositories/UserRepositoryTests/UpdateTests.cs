// <copyright file="UpdateTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

public class UpdateTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    private const string AdminEmailAddress = "test@test.com";

    [Fact]
    [Description("Should update an existing user account")]
    public async Task ShouldUpdateUserAccount()
    {
        // Arrange
        var logger = Substitute.For<ILogger<UsersRepository>>();
        var repository = new UsersRepository(Context, logger);
        var adminUser = await repository.GetSingle(
            x =>
            x.EmailAddress.Equals(AdminEmailAddress),
            TestContext.Current.CancellationToken);

        adminUser.ShouldNotBeNull();
        var user = new UserAccounts
        {
            DisplayName = "Test User",
            FirstName = "Test",
            LastName = "User",
            EmailAddress = "test2@test.com",
            CreatedById = adminUser.Id,
        };
        await Context.UserAccounts.AddAsync(user, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        user.DisplayName = "Updated Name";

        // Act
        var result = await repository.Update(user, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldSatisfyAllConditions(
            x => x.DisplayName.ShouldBe("Updated Name"));

        var savedUser = await Context.UserAccounts.FirstAsync(x => x.EmailAddress.Equals(user.EmailAddress), TestContext.Current.CancellationToken);

        savedUser.ShouldSatisfyAllConditions(
            x => x.DisplayName.ShouldBe("Updated Name"));

        savedUser.ShouldNotBeNull();
        savedUser.DisplayName.ShouldBe("Updated Name");

        logger.ReceivedWithAnyArgs().Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }
}

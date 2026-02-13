// <copyright file="ExceptionTests.cs" company="Defra">
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

public class ExceptionTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    private const string AdminEmailAddress = "test@test.com";

    [Fact]
    [Trait("Category", "Integration")]
    [Description("Should throw duplicate exception")]
    public async Task ShouldThrowDuplicateException()
    {
        // Arrange
        var logger = Substitute.For<ILogger<UsersRepository>>();
        var repository = new UsersRepository(Context, logger);

        var adminUser = await repository.GetSingle(x => x.EmailAddress == AdminEmailAddress, TestContext.Current.CancellationToken);
        adminUser.ShouldNotBeNull();

        var duplicateUser = new UserAccounts
        {
            DisplayName = "Dup User",
            FirstName = "Dup",
            LastName = "User",
            EmailAddress = AdminEmailAddress,   // <-- violates unique constraint om email address field
            CreatedById = adminUser.Id,
        };

        // Act
        Func<Task> act = async () => await repository.Create(duplicateUser);

        // Assert (Shouldly)
        var ex = await act.ShouldThrowAsync<DbUpdateException>();
        ex.InnerException.ShouldNotBeNull(); // often contains provider-specific details

        logger.ReceivedWithAnyArgs().Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }
}

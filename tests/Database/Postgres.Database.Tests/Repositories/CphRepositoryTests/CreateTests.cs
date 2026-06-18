// <copyright file="CreateTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.CphRepositoryTests;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Cphs;
using Defra.Identity.Test.Utilities.Assertions;
using Microsoft.Extensions.Logging;

public class CreateTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Should create a new cph")]
    public async Task ShouldCreateCph()
    {
        // Arrange
        var logger = DefraLoggerExtensions.CreateNSubstituteLogger<CphRepository>();
        var repository = new CphRepository(Context, ReadOnlyContext, logger);

        var adminUser = Context.UserAccounts.First();

        var newCph = new CountyParishHoldings()
        {
            Identifier = "99/001/0001",
            CreatedById = adminUser.Id,
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            ExpiredAt = DateTime.UtcNow.AddDays(-2),
            DeletedById = adminUser.Id,
            DeletedAt = DateTime.UtcNow.AddDays(-1),
        };

        // Act
        var createdCph = await repository
            .Create(newCph, TestContext.Current.CancellationToken);

        // Assert
        createdCph.ShouldSatisfyAllConditions(
            x => x.Identifier.ShouldBe("99/001/0001"),
            x => x.CreatedById.ShouldBe(adminUser.Id),
            x => x.CreatedAt.ShouldBeCloseToUtcNowAddDays(-3),
            x => x.ExpiredAt.ShouldNotBeNull(),
            x => x.ExpiredAt!.Value.ShouldBeCloseToUtcNowAddDays(-2),
            x => x.DeletedById.ShouldBe(adminUser.Id),
            x => x.DeletedAt.ShouldNotBeNull(),
            x => x.DeletedAt!.Value.ShouldBeCloseToUtcNowAddDays(-1));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Creating county parish holding");
    }
}

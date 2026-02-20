// <copyright file="GetSingleTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.CphRepositoryTests;

using System.ComponentModel;
using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Postgres.Database.Tests.Fixtures.SeedData;
using Defra.Identity.Repositories.Cphs;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class GetSingleTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Should get a single entity")]
    public async Task ShouldGetSingleEntity()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphRepository>>();
        var repository = new CphRepository(Context, logger);

        Expression<Func<CountyParishHoldings, bool>> filter = cph => cph.Id == new Guid("088967e7-71b8-457a-9001-5b71f24798fd");

        var adminUser = await SeedDataQueryHelper.GetAdminUser(Context);

        // Act
        var entity = await repository.GetSingle(filter, TestContext.Current.CancellationToken);

        // Assert
        logger.Received(1).Log(LogLevel.Information, "Getting single county parish holding");

        entity.ShouldNotBeNull();

        entity.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("088967e7-71b8-457a-9001-5b71f24798fd")),
            (x) => x.Identifier.ShouldBe("44/000/0007"),
            (x) => x.CreatedAt.ShouldBe(DateTime.Parse("2026-02-07").ToUniversalTime()),
            (x) => x.CreatedById.ShouldBe(adminUser.Id),
            (x) => x.ExpiredAt.ShouldBe(DateTime.Parse("2026-02-12").ToUniversalTime()),
            (x) => x.DeletedAt.ShouldBe(DateTime.Parse("2026-02-13").ToUniversalTime()),
            (x) => x.DeletedById.ShouldBe(adminUser.Id));
    }

    [Fact]
    [Description("Should return null when entity does not exist")]
    public async Task ShouldReturnNullWhenEntityDoesNotExist()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphRepository>>();
        var repository = new CphRepository(Context, logger);

        var noneExistingEntityId = new Guid("e2dd6e69-2866-4065-bbac-b716853889b8");

        Expression<Func<CountyParishHoldings, bool>> filter = cph => cph.Id == noneExistingEntityId;

        // Act
        var entity = await repository.GetSingle(filter, TestContext.Current.CancellationToken);

        // Assert
        logger.Received(1).Log(LogLevel.Information, "Getting single county parish holding");

        entity.ShouldBeNull();
    }
}

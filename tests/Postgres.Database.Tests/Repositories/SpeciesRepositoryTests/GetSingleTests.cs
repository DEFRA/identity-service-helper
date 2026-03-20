// <copyright file="GetSingleTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.SpeciesRepositoryTests;

using System.ComponentModel;
using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Cphs;
using Defra.Identity.Repositories.Species;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class GetSingleTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Should get a single species")]
    public async Task ShouldGetSingleSpecies()
    {
        // Arrange
        var logger = Substitute.For<ILogger<AnimalSpeciesRepository>>();
        var repository = new AnimalSpeciesRepository(Context, ReadOnlyContext, logger);

        Expression<Func<AnimalSpecies, bool>> filter = species => species.Id == "CTT";

        // Act
        var entity = await repository.GetSingle(filter, TestContext.Current.CancellationToken);

        // Assert
        logger.Received(1).Log(LogLevel.Information, "Getting single animal species");

        entity.ShouldNotBeNull();

        entity.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe("CTT"),
            (x) => x.IsActive.ShouldBeTrue());
    }

    [Fact]
    [Description("Should return null when species does not exist")]
    public async Task ShouldReturnNullWhenSpeciesDoesNotExist()
    {
        // Arrange
        var logger = Substitute.For<ILogger<AnimalSpeciesRepository>>();
        var repository = new AnimalSpeciesRepository(Context, ReadOnlyContext, logger);

        var noneExistingSpecoesId = "FAKE";

        Expression<Func<AnimalSpecies, bool>> filter = species => species.Id == noneExistingSpecoesId;

        // Act
        var entity = await repository.GetSingle(filter, TestContext.Current.CancellationToken);

        // Assert
        logger.Received(1).Log(LogLevel.Information, "Getting single animal species");

        entity.ShouldBeNull();
    }
}

// <copyright file="GetAllTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.SpeciesRepositoryTests;

using System.ComponentModel;
using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Species;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class GetAllTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Should get all species that are active")]
    public async Task ShouldGetAllSpeciesThatAreActive()
    {
        // Arrange
        var logger = Substitute.For<ILogger<AnimalSpeciesRepository>>();
        var repository = new AnimalSpeciesRepository(Context, logger);

        Expression<Func<AnimalSpecies, bool>> filter = species => species.IsActive;

        // Act
        var speciesList = await repository.GetList(filter, TestContext.Current.CancellationToken);

        // Assert
        logger.Received(1).Log(LogLevel.Information, "Getting list of animal species");

        speciesList.ShouldSatisfyAllConditions((x) => x.Count.ShouldBe(1));

        var firstItem = speciesList[0];
        firstItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe("CTT"),
            (x) => x.IsActive.ShouldBeTrue());
    }

    [Fact]
    [Description("Should get all species that are both active and inactive")]
    public async Task ShouldGetAllSpeciesThatAreBothActiveAndInactive()
    {
        // Arrange
        var logger = Substitute.For<ILogger<AnimalSpeciesRepository>>();
        var repository = new AnimalSpeciesRepository(Context, logger);

        Expression<Func<AnimalSpecies, bool>> filter = species => true;

        // Act
        var speciesList = await repository.GetList(filter, TestContext.Current.CancellationToken);

        // Assert
        logger.Received(1).Log(LogLevel.Information, "Getting list of animal species");

        speciesList.ShouldSatisfyAllConditions((x) => x.Count.ShouldBe(6));

        var activeSpecies = speciesList.First(x => x.IsActive);
        var inactiveSpecies = speciesList.First(x => !x.IsActive);
        activeSpecies.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe("CTT"),
            (x) => x.IsActive.ShouldBeTrue());
        inactiveSpecies.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldNotBe("CTT"),
            (x) => x.IsActive.ShouldBeFalse());
    }
}

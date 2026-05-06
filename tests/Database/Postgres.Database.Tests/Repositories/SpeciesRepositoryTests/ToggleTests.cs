// <copyright file="ToggleTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.SpeciesRepositoryTests;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Species;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class ToggleTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Should update an existing species")]
    public async Task ShouldUpdateSpecies()
    {
        // Arrange
        var logger = Substitute.For<ILogger<AnimalSpeciesRepository>>();
        var repository = new AnimalSpeciesRepository(Context, ReadOnlyContext, logger);
        const string id = "CTT";

        var speciesToUpdate = await repository.GetSingle(x => x.Id == id, TestContext.Current.CancellationToken);
        speciesToUpdate.ShouldNotBeNull();

        // Act
        speciesToUpdate.IsActive = false;
        var updatedSpecies = await repository.Update(speciesToUpdate, TestContext.Current.CancellationToken);
        var speciesFromRequery = await repository.GetSingle(x => x.Id == id, TestContext.Current.CancellationToken);

        // Assert
        updatedSpecies.ShouldNotBeNull();
        speciesFromRequery.ShouldNotBeNull();

        updatedSpecies.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(id),
            (x) => x.IsActive.ShouldBeFalse());

        speciesFromRequery.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(id),
            (x) => x.IsActive.ShouldBeFalse());
    }

    [Fact]
    [Description("Should throw when species does not exist")]
    public async Task ShouldThrowWhenSpeciesDoesNotExist()
    {
        // Arrange
        var logger = Substitute.For<ILogger<AnimalSpeciesRepository>>();
        var repository = new AnimalSpeciesRepository(Context, ReadOnlyContext, logger);
        const string id = "FAKE";

        var speciesToUpdate = new AnimalSpecies() { Id = id, Name = "Not Real", IsActive = false, };

        // Act/Assert
        await Should.ThrowAsync<DbUpdateException>(() => repository.Update(speciesToUpdate, TestContext.Current.CancellationToken));
    }
}

// <copyright file="SpeciesMapperTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Common.Mappers;

using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Services.Common.Mappers;

public class SpeciesMapperTests
{
    [Fact]
    public void SpeciesMapper_ReturnsValidInstance()
    {
        // Arrange
        var species = new AnimalSpecies() { Id = "SPC", Name = "Species", IsActive = true };

        // Act
        var result = SpeciesMapper.MapAnimalSpeciesEntityToAnimalSpecies(species);

        // Assert
        result.ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(species));
    }
}

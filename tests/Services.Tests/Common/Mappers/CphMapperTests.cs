// <copyright file="CphMapperTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Common.Mappers;

using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Services.Common.Mappers;

public class CphMapperTests
{
    [Fact]
    public void CphMapper_ReturnsValidInstance()
    {
        // Arrange
        var ctt = new AnimalSpecies() { Id = "CTT", Name = "Cattle", IsActive = true, };
        var gt = new AnimalSpecies() { Id = "GT", Name = "Goat", IsActive = false, };

        var cph = new CountyParishHoldings
        {
            Id = Guid.NewGuid(),
            Identifier = "12/122/1223",
            ExpiredAt = DateTime.UtcNow,
            CreatedById = Guid.NewGuid(),
            CreatedByUser = new UserAccounts(),
            CreatedAt = DateTime.UtcNow,
            DeletedById = Guid.NewGuid(),
            DeletedByUser = new UserAccounts(),
            DeletedAt = DateTime.UtcNow,
        };

        cph.CountyParishHoldingAnimalSpecies.Add(new CountyParishHoldingAnimalSpecies
        {
            AnimalSpeciesId = ctt.Id, AnimalSpecies = ctt, CountyParishHolding = cph,
        });

        cph.CountyParishHoldingAnimalSpecies.Add(new CountyParishHoldingAnimalSpecies
        {
            AnimalSpeciesId = gt.Id, AnimalSpecies = gt, CountyParishHolding = cph,
        });

        // Act
        var result = CphMapper.MapCphEntityToCph(cph);

        // Assert
        result.ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(cph));
    }
}

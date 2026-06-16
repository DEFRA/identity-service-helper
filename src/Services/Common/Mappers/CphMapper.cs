// <copyright file="CphMapper.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Mappers;

using Defra.Identity.Models.Responses.Cphs;
using Defra.Identity.Postgres.Database.Entities;

public static class CphMapper
{
    public static Cph MapCphEntityToCph(CountyParishHoldings cphEntity)
    {
        return new Cph
        {
            Id = cphEntity.Id,
            CountyParishHoldingNumber = cphEntity.Identifier,
            AllowedSpecies = cphEntity.CountyParishHoldingAnimalSpecies
                .Select(allowedSpecies =>
                    SpeciesMapper.MapAnimalSpeciesEntityToAnimalSpecies(allowedSpecies.AnimalSpecies)),
            ExpiredAt = cphEntity.ExpiredAt,
            Expired = cphEntity.ExpiredAt != null,
        };
    }
}

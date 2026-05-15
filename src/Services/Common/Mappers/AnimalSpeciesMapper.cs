// <copyright file="AnimalSpeciesMapper.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Mappers;

using ModelAnimalSpecies = Defra.Identity.Postgres.Database.Entities.AnimalSpecies;
using ResponseAnimalSpecies = Defra.Identity.Models.Responses.Species.AnimalSpecies;

public static class AnimalSpeciesMapper
{
    public static ResponseAnimalSpecies MapAnimalSpeciesEntityToAnimalSpecies(ModelAnimalSpecies animalSpeciesEntity)
    {
        return new ResponseAnimalSpecies()
        {
            Id = animalSpeciesEntity.Id, Name = animalSpeciesEntity.Name, IsActive = animalSpeciesEntity.IsActive,
        };
    }
}

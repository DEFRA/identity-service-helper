// <copyright file="IAnimalSpeciesRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Species;

public interface IAnimalSpeciesRepository :
    IGetListRepository<Postgres.Database.Entities.AnimalSpecies>,
    IGetSingleRepository<Postgres.Database.Entities.AnimalSpecies>,
    IUpdateRepository<Postgres.Database.Entities.AnimalSpecies>;

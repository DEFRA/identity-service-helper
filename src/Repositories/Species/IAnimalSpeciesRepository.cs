// <copyright file="IAnimalSpeciesRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Species;

using Defra.Identity.Repositories.Common.Composites;

public interface IAnimalSpeciesRepository :
    IListable<Postgres.Database.Entities.AnimalSpecies>,
    IGettable<Postgres.Database.Entities.AnimalSpecies>,
    IUpdatable<Postgres.Database.Entities.AnimalSpecies>;

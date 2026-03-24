// <copyright file="ICphDelegationsRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Delegations;

using Defra.Identity.Postgres.Database.Entities;

public interface ICphDelegationsRepository :
    IRepository<CountyParishHoldingDelegations>;

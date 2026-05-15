// <copyright file="ICphDelegationsRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Delegations;

using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Composites;

public interface ICphDelegationsRepository :
    IGettable<CountyParishHoldingDelegations>,
    IListable<CountyParishHoldingDelegations>,
    ICreatable<CountyParishHoldingDelegations>,
    IUpdatable<CountyParishHoldingDelegations>;

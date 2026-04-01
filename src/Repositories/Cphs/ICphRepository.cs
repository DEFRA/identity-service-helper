// <copyright file="ICphRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Cphs;

using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Composites;

public interface ICphRepository : IGettable<CountyParishHoldings>, IPageable<CountyParishHoldings>, IUpdatable<CountyParishHoldings>, ICreatable<CountyParishHoldings>;

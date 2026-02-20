// <copyright file="ICphRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Cphs;

using Defra.Identity.Postgres.Database.Entities;

public interface ICphRepository : IPageableRepository<CountyParishHoldings>;

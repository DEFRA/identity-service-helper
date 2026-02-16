// <copyright file="IDelegatesRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Delegates;

using Defra.Identity.Postgres.Database.Entities;

public interface IDelegatesRepository : IRepository<Delegations>;

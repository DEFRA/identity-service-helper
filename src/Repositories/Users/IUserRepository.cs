// <copyright file="IUserRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Users;

using Defra.Identity.Postgres.Database.Entities;

public interface IUserRepository : IRepository<UserAccounts>;

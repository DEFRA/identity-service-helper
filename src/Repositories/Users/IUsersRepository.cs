// <copyright file="IUsersRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Users;

using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Composites;

public interface IUsersRepository :
    IReference,
    IRepository<UserAccounts>;

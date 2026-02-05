// <copyright file="IUsersRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Users;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;

public interface IUsersRepository : IRepository<UserAccount>
{
    Task<bool> Suspend(Expression<Func<UserAccount, bool>> predicate, CancellationToken cancellationToken = default);

    Task<bool> Activate(Expression<Func<UserAccount, bool>> predicate, CancellationToken cancellationToken = default);
}

// <copyright file="UserRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Users;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Microsoft.Extensions.Logging;

/// <inheritdoc />
public partial class UserRepository(
    PostgresDbContext context,
    ReadOnlyPostgresDbContext readOnlyContext,
    ILogger<UserRepository> logger)
    : IUserRepository
{
    public async Task<UserAccounts?> GetSingle(
        Expression<Func<UserAccounts, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        LogGettingSingleUserAccount();
        var query = await readOnlyContext.UserAccounts
            .SingleOrDefaultAsync(predicate, cancellationToken);

        return query;
    }

    public async Task<List<UserAccounts>> GetList(
        Expression<Func<UserAccounts, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        LogGettingListOfUserAccounts();
        var query = await readOnlyContext.UserAccounts
            .Where(predicate).ToListAsync<UserAccounts>(cancellationToken);

        return query;
    }

    public async Task<UserAccounts> Create(
        UserAccounts entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        LogCreatingUserAccount();

        var addedEntry = await context.UserAccounts.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return addedEntry.Entity;
    }

    public async Task<UserAccounts> Update(
        UserAccounts entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        LogUpdatingUserAccountWithId(entity.Id);
        context.Update(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }
}

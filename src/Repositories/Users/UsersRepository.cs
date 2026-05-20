// <copyright file="UsersRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Users;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Microsoft.Extensions.Logging;

public partial class UsersRepository(
    PostgresDbContext context,
    ReadOnlyPostgresDbContext readOnlyContext,
    ILogger<UsersRepository> logger)
    : IUsersRepository
{
    public async Task<bool> ValidateReferenceById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        LogValidatingUserAccountReferenceWithId(id);

        var entity = await readOnlyContext.UserAccounts
            .SingleOrDefaultAsync((entity) => entity.Id == id, cancellationToken);

        return entity is { DeletedAt: null };
    }

    public async Task<List<UserAccounts>> GetAll()
    {
        LogGettingAllUserAccounts();
        var query = readOnlyContext.UserAccounts.AsQueryable();
        return await query.ToListAsync();
    }

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

        LogCreatingUserAccountWithId(entity.Id);
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

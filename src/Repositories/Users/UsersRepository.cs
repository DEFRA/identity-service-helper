// <copyright file="UsersRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Users;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Exceptions;

public class UsersRepository(PostgresDbContext context)
    : IUsersRepository
{
    public async Task<List<UserAccounts>> GetAll()
    {
        var query = context.UserAccounts.AsQueryable();
        return await query.ToListAsync();
    }

    public async Task<UserAccounts?> GetSingle(Expression<Func<UserAccounts, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var query = await context.UserAccounts
            .SingleOrDefaultAsync(predicate, cancellationToken);

        return query;
    }

    public async Task<List<UserAccounts>> GetList(Expression<Func<UserAccounts, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var query = await context.UserAccounts
            .Where(predicate).ToListAsync<UserAccounts>(cancellationToken);

        return query;
    }

    public async Task<UserAccounts> Create(UserAccounts entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var addedEntry = await context.UserAccounts.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return addedEntry.Entity;
    }

    public async Task<UserAccounts> Update(UserAccounts entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        context.Update(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> Delete(Expression<Func<UserAccounts, bool>> predicate,  Guid operatorId, CancellationToken cancellationToken = default)
    {
        var userAccount = await context.UserAccounts
            .SingleOrDefaultAsync(predicate, cancellationToken);

        if (userAccount == null)
        {
            throw new NotFoundException("User account not found");
        }

        userAccount.IsDeleted = true;
        userAccount.DeletedById = operatorId;
        userAccount.DeletedAt = DateTime.UtcNow;
        context.UserAccounts.Update(userAccount);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}

// <copyright file="UsersRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Users;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Exceptions;
using Microsoft.Extensions.Logging;

public class UsersRepository(PostgresDbContext context, ILogger<UsersRepository> logger)
    : IUsersRepository
{
    public async Task<List<UserAccounts>> GetAll()
    {
        logger.LogInformation("Getting all user accounts");
        var query = context.UserAccounts.AsQueryable();
        return await query.ToListAsync();
    }

    public async Task<UserAccounts?> GetSingle(Expression<Func<UserAccounts, bool>> predicate, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting single user account");
        var query = await context.UserAccounts
            .SingleOrDefaultAsync(predicate, cancellationToken);

        return query;
    }

    public async Task<List<UserAccounts>> GetList(Expression<Func<UserAccounts, bool>> predicate, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting list of user accounts");
        var query = await context.UserAccounts
            .Where(predicate).ToListAsync<UserAccounts>(cancellationToken);

        return query;
    }

    public async Task<UserAccounts> Create(UserAccounts entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        logger.LogInformation("Creating user account with id {Id}", entity.Id);
        var addedEntry = await context.UserAccounts.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return addedEntry.Entity;
    }

    public async Task<UserAccounts> Update(UserAccounts entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        logger.LogInformation("Updating user account with id {Id}", entity.Id);
        context.Update(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> Delete(Expression<Func<UserAccounts, bool>> predicate,  Guid operatorId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting user account with operator id {OperatorId}", operatorId);
        var userAccount = await context.UserAccounts
            .SingleOrDefaultAsync(predicate, cancellationToken);

        if (userAccount == null)
        {
            logger.LogWarning("User account not found for deletion");
            throw new NotFoundException("User account not found");
        }

        userAccount.DeletedById = operatorId;
        userAccount.DeletedAt = DateTime.UtcNow;
        context.UserAccounts.Update(userAccount);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}

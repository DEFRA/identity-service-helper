// <copyright file="UsersRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

using Defra.Identity.Repositories.Exceptions;

namespace Defra.Identity.Repositories.Users;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;

public class UsersRepository(AuthContext context)
    : IUsersRepository
{
    public async Task<List<UserAccount>> GetAll()
    {
        var query = context.Users.Include(x => x.Status).AsQueryable();
        return await query.ToListAsync();
    }

    public async Task<UserAccount?> GetSingle(Expression<Func<UserAccount, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var query = await context.Users
            .Include(x => x.Status)
            .SingleOrDefaultAsync(predicate, cancellationToken);

        return query;
    }

    public async Task<List<UserAccount>> GetList(Expression<Func<UserAccount, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var query = await context.Users
            .Include(x => x.Status)
            .Where(predicate).ToListAsync<UserAccount>(cancellationToken);

        return query;
    }

    public async Task<UserAccount> Create(UserAccount entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var addedEntry = await context.Users.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        await context.Entry(addedEntry.Entity)
            .Reference(x => x.Status)
            .LoadAsync(cancellationToken);

        return addedEntry.Entity;
    }

    public async Task<UserAccount> Update(UserAccount entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        context.Update(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> Delete(Expression<Func<UserAccount, bool>> predicate,  Guid operatorId, CancellationToken cancellationToken = default)
    {
        var userAccount = await context.Users
            .SingleOrDefaultAsync(predicate, cancellationToken);

        if (userAccount == null)
        {
            throw new NotFoundException("User account not found");
        }

        userAccount.StatusTypeId = 4;
        userAccount.UpdatedBy = userAccount.Id;
        context.Users.Update(userAccount);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> Suspend(Expression<Func<UserAccount, bool>> predicate, Guid operatorId, CancellationToken cancellationToken = default)
    {
        var userAccount = await context.Users
            .SingleOrDefaultAsync(predicate, cancellationToken);
        if (userAccount == null)
        {
            throw new NotFoundException("User account not found");
        }

        userAccount.StatusTypeId = 3;
        userAccount.UpdatedBy = operatorId;

        context.Users.Update(userAccount);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> Activate(Expression<Func<UserAccount, bool>> predicate,  Guid operatorId, CancellationToken cancellationToken = default)
    {
        var userAccount = await context.Users
            .SingleOrDefaultAsync(predicate, cancellationToken);
        if (userAccount == null)
        {
            throw new NotFoundException("User account not found");
        }

        userAccount.StatusTypeId = 2;
        userAccount.UpdatedBy = operatorId;
        context.Users.Update(userAccount);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

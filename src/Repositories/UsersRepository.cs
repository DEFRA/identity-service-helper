// <copyright file="UsersRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;

public class UsersRepository(AuthContext context)
    : IRepository<UserAccount>
{
    public async Task<List<UserAccount>> GetAll()
    {
        var query = context.Users.AsQueryable();
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
        await context.Users.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<UserAccount> Update(UserAccount entity, CancellationToken cancellationToken = default)
    {
        context.Users.Update(entity);
        await context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public Task<bool> Delete(Func<UserAccount, bool> predicate)
    {
        throw new NotImplementedException();
    }
}

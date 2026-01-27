// <copyright file="UsersRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services;

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

    public async Task<UserAccount?> Get(Expression<Func<UserAccount, bool>> predicate)
    {
        var query = await context.Users.SingleOrDefaultAsync(predicate);
        return query ?? null;
    }

    public async Task<UserAccount> Create(UserAccount entity)
    {
        await context.Users.AddAsync(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<UserAccount> Update(UserAccount entity)
    {
        context.Users.Update(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public Task<bool> Delete(Func<UserAccount, bool> predicate)
    {
        throw new NotImplementedException();
    }
}

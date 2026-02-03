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

        var existing = await context.Users
            .Include(x => x.Status)
            .SingleOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);

        if (existing is null)
        {
            throw new InvalidOperationException($"User with id '{entity.Id}' was not found.");
        }

        // Update scalar fields (examples — adjust to your real model)
        existing.EmailAddress = entity.EmailAddress;
        existing.FirstName = entity.FirstName;
        existing.LastName = entity.LastName;
        existing.DisplayName = entity.DisplayName;

        // If your incoming "entity" carries a status *string* somewhere,
        // resolve it to the Status entity and set the relationship.
        //
        // Example assumes you temporarily store the incoming status name in entity.Status.Name
        // or another field. Replace `incomingStatusName` with where your string actually is.
        var incomingStatusName = entity.Status?.Name;

        if (!string.IsNullOrWhiteSpace(incomingStatusName))
        {
            var status = await context.StatusTypes
                .SingleOrDefaultAsync(s => s.Name.Equals(incomingStatusName), cancellationToken);

            if (status is null)
            {
                throw new InvalidOperationException($"Unknown status '{incomingStatusName}'.");
            }

            existing.Status.Id = status.Id; // alternatively: existing.StatusId = status.Id;
        }

        await context.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public Task<bool> Delete(Func<UserAccount, bool> predicate)
    {
        throw new NotImplementedException();
    }
}

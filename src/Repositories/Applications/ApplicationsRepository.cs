// <copyright file="ApplicationsRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Applications;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Microsoft.Extensions.Logging;

public partial class ApplicationsRepository(
    PostgresDbContext context,
    ReadOnlyPostgresDbContext readOnlyContext,
    ILogger<ApplicationsRepository> logger)
    : IApplicationsRepository
{
    public async Task<Applications?> GetSingle(Expression<Func<Applications, bool>> predicate, CancellationToken cancellationToken = default)
    {
        LogGettingSingleApplication();
        var query = await readOnlyContext.Applications
            .SingleOrDefaultAsync(predicate, cancellationToken);

        return query;
    }

    public async Task<List<Applications>> GetList(Expression<Func<Applications, bool>> predicate, CancellationToken cancellationToken = default)
    {
        LogGettingListOfApplications();
        var query = await readOnlyContext.Applications
            .Where(predicate).ToListAsync<Applications>(cancellationToken);

        return query;
    }

    public async Task<Applications> Create(Applications entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        LogCreatingApplicationWithId(entity.Id);
        var addedEntry = await context.Applications.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return addedEntry.Entity;
    }

    public async Task<Applications> Update(Applications entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        LogUpdatingApplicationWithId(entity.Id);
        context.Update(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }
}

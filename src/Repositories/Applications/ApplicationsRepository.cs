// <copyright file="ApplicationsRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Applications;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Microsoft.Extensions.Logging;

public class ApplicationsRepository(PostgresDbContext context, ILogger<ApplicationsRepository> logger) : IApplicationsRepository
{
    public async Task<Applications?> GetSingle(Expression<Func<Applications, bool>> predicate, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting single application");
        var query = await context.Applications
            .SingleOrDefaultAsync(predicate, cancellationToken);

        return query;
    }

    public async Task<List<Applications>> GetList(Expression<Func<Applications, bool>> predicate, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting list of applications");
        var query = await context.Applications
            .Where(predicate).ToListAsync<Applications>(cancellationToken);

        return query;
    }

    public async Task<Applications> Create(Applications entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        logger.LogInformation("Creating application with id {Id}", entity.Id);
        var addedEntry = await context.Applications.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return addedEntry.Entity;
    }

    public async Task<Applications> Update(Applications entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        logger.LogInformation("Updating application with id {Id}", entity.Id);
        context.Update(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> Delete(Expression<Func<Applications, bool>> predicate, Guid operatorId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting application with operator id {OperatorId}", operatorId);
        var application = await context.Applications
            .SingleOrDefaultAsync(predicate, cancellationToken);

        if (application == null)
        {
            logger.LogWarning("Application not found for deletion");
            throw new Exceptions.NotFoundException("Application not found");
        }

        application.DeletedById = operatorId;
        application.DeletedAt = DateTime.UtcNow;
        context.Applications.Update(application);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}

// <copyright file="CountyParishHoldingDelegationsNotificationsRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Messaging;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Exceptions;
using Microsoft.Extensions.Logging;

public class CountyParishHoldingDelegationsNotificationsRepository(
    PostgresDbContext context,
    ReadOnlyPostgresDbContext readOnlyContext,
    ILogger<CountyParishHoldingDelegationsNotificationsRepository> logger)
    : ICountyParishHoldingDelegationsNotificationsRepository
{
    public async Task<CountyParishHoldingDelegationsNotifications?> GetSingle(
        Expression<Func<CountyParishHoldingDelegationsNotifications, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting single delegation notification");

        var result = await readOnlyContext.CountyParishHoldingDelegationsNotifications
            .Include(x => x.Delegation)
            .Include(x => x.Message)
            .SingleOrDefaultAsync(predicate, cancellationToken);

        return result;
    }

    public async Task<List<CountyParishHoldingDelegationsNotifications>> GetList(
        Expression<Func<CountyParishHoldingDelegationsNotifications, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting list of delegation notifications");

        var results = await readOnlyContext.CountyParishHoldingDelegationsNotifications
            .Include(x => x.Delegation)
            .Include(x => x.Message)
            .Where(predicate)
            .ToListAsync(cancellationToken);

        return results;
    }

    public async Task<CountyParishHoldingDelegationsNotifications> Create(
        CountyParishHoldingDelegationsNotifications entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        logger.LogInformation(
            "Creating delegation notification for delegation {DelegationId} and message {MessageId}",
            entity.DelegationId,
            entity.MessageId);

        var addedEntry = await context.CountyParishHoldingDelegationsNotifications.AddAsync(
            entity,
            cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result = await readOnlyContext.CountyParishHoldingDelegationsNotifications
            .Include(x => x.Delegation)
            .Include(x => x.Message)
            .SingleAsync(
                x => x.DelegationId == addedEntry.Entity.DelegationId &&
                    x.MessageId == addedEntry.Entity.MessageId,
                cancellationToken);

        return result;
    }

    public async Task<CountyParishHoldingDelegationsNotifications> Update(
        CountyParishHoldingDelegationsNotifications entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        logger.LogInformation(
            "Updating delegation notification for delegation {DelegationId} and message {MessageId}",
            entity.DelegationId,
            entity.MessageId);

        context.Update(entity);
        await context.SaveChangesAsync(cancellationToken);

        var result = await readOnlyContext.CountyParishHoldingDelegationsNotifications
            .Include(x => x.Delegation)
            .Include(x => x.Message)
            .SingleAsync(
                x => x.DelegationId == entity.DelegationId && x.MessageId == entity.MessageId,
                cancellationToken);

        return result;
    }

    public async Task<bool> Delete(
        Expression<Func<CountyParishHoldingDelegationsNotifications, bool>> predicate,
        Guid operatorId,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Deleting delegation notification with operator id {OperatorId}",
            operatorId);

        var entity = await context.CountyParishHoldingDelegationsNotifications
            .SingleOrDefaultAsync(predicate, cancellationToken);

        if (entity == null)
        {
            logger.LogWarning("Delegation notification not found for deletion");
            throw new NotFoundException("Delegation notification not found");
        }

        context.CountyParishHoldingDelegationsNotifications.Remove(entity);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}

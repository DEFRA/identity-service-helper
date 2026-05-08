// <copyright file="ExternalMessagingRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Messaging;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Exceptions;
using Microsoft.Extensions.Logging;

public partial class ExternalMessagingRepository(
    PostgresDbContext context,
    ReadOnlyPostgresDbContext readOnlyContext,
    ILogger<ExternalMessagingRepository> logger)
    : IExternalMessagingRepository
{
    public async Task<ExternalMessaging?> GetSingle(
        Expression<Func<ExternalMessaging, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        LogGettingSingleExternalMessagingRecord();

        var result = await readOnlyContext
            .ExternalMessaging
            .Include(x => x.CreatedByUser)
            .Include(x => x.CountyParishHoldingDelegationsNotifications)
            .SingleOrDefaultAsync(predicate, cancellationToken);

        return result;
    }

    public async Task<List<ExternalMessaging>> GetList(
        Expression<Func<ExternalMessaging, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        LogGettingListOfExternalMessagingRecords();

        var results = await readOnlyContext
            .ExternalMessaging
            .Include(x => x.CreatedByUser)
            .Include(x => x.CountyParishHoldingDelegationsNotifications)
            .Where(predicate)
            .ToListAsync(cancellationToken);

        return results;
    }

    public async Task<ExternalMessaging> Create(ExternalMessaging entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        LogCreatingExternalMessagingRecordWithId(entity.Id);
        var addedEntry = await context.ExternalMessaging.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result = await readOnlyContext
            .ExternalMessaging
            .Include(x => x.CreatedByUser)
            .Include(x => x.CountyParishHoldingDelegationsNotifications)
            .SingleAsync(x => x.Id == addedEntry.Entity.Id, cancellationToken);

        return result;
    }

    public async Task<ExternalMessaging> Update(ExternalMessaging entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        LogUpdatingExternalMessagingRecordWithId(entity.Id);

        var trackedEntity =
            context.ExternalMessaging.Local
                .FirstOrDefault(x => x.Id == entity.Id) ??
            await context.ExternalMessaging
                .SingleAsync(x => x.Id == entity.Id, cancellationToken);

        context.Entry(trackedEntity).CurrentValues.SetValues(entity);
        await context.SaveChangesAsync(cancellationToken);

        var result = await readOnlyContext
            .ExternalMessaging
            .Include(x => x.CreatedByUser)
            .Include(x => x.CountyParishHoldingDelegationsNotifications)
            .SingleAsync(x => x.Id == entity.Id, cancellationToken);

        return result;
    }

    public async Task<bool> Delete(
        Expression<Func<ExternalMessaging, bool>> predicate,
        Guid operatorId,
        CancellationToken cancellationToken = default)
    {
        LogDeletingExternalMessagingRecordWithOperatorId(operatorId);

        var entity = await context.ExternalMessaging
            .SingleOrDefaultAsync(predicate, cancellationToken);

        if (entity == null)
        {
            LogExternalMessagingRecordNotFoundForDeletion();
            throw new NotFoundException("External messaging record not found");
        }

        context.ExternalMessaging.Remove(entity);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}

// <copyright file="CountyParishHoldingDelegationsNotificationsRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Messaging;

using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Microsoft.Extensions.Logging;

/// <inheritdoc />
public partial class CountyParishHoldingDelegationsNotificationsRepository(
    PostgresDbContext context,
    ReadOnlyPostgresDbContext readOnlyContext,
    ILogger<CountyParishHoldingDelegationsNotificationsRepository> logger)
    : ICountyParishHoldingDelegationsNotificationsRepository
{
    public async Task<CountyParishHoldingDelegationsNotifications> Create(
        CountyParishHoldingDelegationsNotifications entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        LogCreatingDelegationNotificationForDelegationAndMessage(entity.DelegationId, entity.MessageId);

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
}

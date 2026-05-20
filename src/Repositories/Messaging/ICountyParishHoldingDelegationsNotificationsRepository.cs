// <copyright file="ICountyParishHoldingDelegationsNotificationsRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Messaging;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;

public interface ICountyParishHoldingDelegationsNotificationsRepository :
    IRepository<CountyParishHoldingDelegationsNotifications>
{
    Task<bool> Delete(
        Expression<Func<CountyParishHoldingDelegationsNotifications, bool>> predicate,
        Guid operatorId,
        CancellationToken cancellationToken = default);
}

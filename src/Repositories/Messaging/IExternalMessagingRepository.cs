// <copyright file="IExternalMessagingRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Messaging;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;

public interface IExternalMessagingRepository :
    IRepository<ExternalMessaging>
{
    Task<bool> Delete(
        Expression<Func<ExternalMessaging, bool>> predicate,
        Guid operatorId,
        CancellationToken cancellationToken = default);
}

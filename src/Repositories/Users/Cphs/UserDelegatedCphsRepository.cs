// <copyright file="UserDelegatedCphsRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Users.Cphs;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Exceptions;
using Microsoft.Extensions.Logging;

public class UserDelegatedCphsRepository(
    ReadOnlyPostgresDbContext readOnlyContext,
    ILogger<UserAssociatedCphsRepository> logger) : IUserDelegatedCphsRepository
{
    public async Task<List<CountyParishHoldingDelegations>> GetList(
        Expression<Func<UserAccounts, bool>> primaryPredicate,
        Expression<Func<CountyParishHoldingDelegations, bool>> associationsPredicate,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting list of delegated cphs for user account");

        var primaryEntity = await readOnlyContext.UserAccounts
            .FirstOrDefaultAsync(primaryPredicate, cancellationToken);

        if (primaryEntity == null)
        {
            throw new NotFoundException("User account not found.");
        }

        var results = await readOnlyContext.CountyParishHoldingDelegations
            .Include(p => p.CountyParishHolding)
            .Include(p => p.DelegatingUser)
            .Include(p => p.DelegatedUserRole)
            .Where(entity => entity.DelegatedUserId == primaryEntity.Id)
            .Where(associationsPredicate)
            .ToListAsync(cancellationToken);

        return results;
    }
}

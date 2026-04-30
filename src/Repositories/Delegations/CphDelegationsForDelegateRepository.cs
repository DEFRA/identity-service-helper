// <copyright file="CphDelegationsForDelegateRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Delegations;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Exceptions;
using Microsoft.Extensions.Logging;

public class CphDelegationsForDelegateRepository(
    ReadOnlyPostgresDbContext readOnlyContext,
    ILogger<CphDelegationsForDelegateRepository> logger) : ICphDelegationsForDelegateRepository
{
    public async Task<List<CountyParishHoldingDelegations>> GetList(
        Expression<Func<UserAccounts, bool>> primaryPredicate,
        Expression<Func<CountyParishHoldingDelegations, bool>> associationsPredicate,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting list of delegations for delegate");

        var primaryEntity = await readOnlyContext.UserAccounts
            .FirstOrDefaultAsync(primaryPredicate, cancellationToken);

        if (primaryEntity == null)
        {
            throw new NotFoundException("User account not found.");
        }

        var results = await readOnlyContext.CountyParishHoldingDelegations
            .Include(p => p.CountyParishHolding)
            .Include(p => p.DelegatingUser)
            .Include(p => p.DelegatedUser)
            .Include(p => p.DelegatedUserRole)
            .Where(entity => entity.DelegatedUserId == primaryEntity.Id)
            .Where(associationsPredicate)
            .ToListAsync(cancellationToken);

        return results;
    }
}

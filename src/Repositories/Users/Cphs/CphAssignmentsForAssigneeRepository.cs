// <copyright file="CphAssignmentsForAssigneeRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Users.Cphs;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Exceptions;
using Microsoft.Extensions.Logging;

public class CphAssignmentsForAssigneeRepository(
    ReadOnlyPostgresDbContext readOnlyContext,
    ILogger<CphAssignmentsForAssigneeRepository> logger) : ICphAssignmentsForAssigneeRepository
{
    public async Task<List<ApplicationUserAccountHoldingAssignments>> GetList(
        Expression<Func<UserAccounts, bool>> primaryPredicate,
        Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> associationsPredicate,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting list of cphs for user account");

        var primaryEntity = await readOnlyContext.UserAccounts
            .FirstOrDefaultAsync(primaryPredicate, cancellationToken);

        if (primaryEntity == null)
        {
            throw new NotFoundException("User account not found.");
        }

        var results = await readOnlyContext.Entry(primaryEntity)
            .Collection(p => p.ApplicationUserAccountHoldingAssignments)
            .Query()
            .Include(p => p.CountyParishHolding)
            .Include(p => p.Role)
            .Where(associationsPredicate)
            .ToListAsync(cancellationToken);

        return results;
    }
}

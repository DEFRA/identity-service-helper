// <copyright file="CphAssignmentsRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Assignments;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common;
using Defra.Identity.Repositories.Common.Exceptions;
using Microsoft.Extensions.Logging;

public class CphAssignmentsRepository(
    ReadOnlyPostgresDbContext readOnlyContext,
    ILogger<CphAssignmentsRepository> logger) : ICphAssignmentsRepository
{
    public async Task<PagedEntities<ApplicationUserAccountHoldingAssignments>> GetPaged<TOrderBy>(
        Expression<Func<CountyParishHoldings, bool>> primaryPredicate,
        Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> associationsPredicate,
        int pageNumber,
        int pageSize,
        Expression<Func<ApplicationUserAccountHoldingAssignments, TOrderBy>> orderBy,
        bool orderByDescending,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting list of assignments for county parish holding");

        var primaryEntity = await readOnlyContext.CountyParishHoldings
            .FirstOrDefaultAsync(primaryPredicate, cancellationToken);

        if (primaryEntity == null)
        {
            throw new NotFoundException("County parish holding not found.");
        }

        var pagedResults = await readOnlyContext
            .Entry(primaryEntity)
            .Collection(p => p.ApplicationUserAccountHoldingAssignments)
            .Query()
            .Include(p => p.CountyParishHolding)
            .Include(p => p.UserAccount)
            .Include(p => p.Role)
            .Where(associationsPredicate)
            .ToPaged(pageNumber, pageSize, orderBy, orderByDescending, cancellationToken);

        return pagedResults;
    }
}

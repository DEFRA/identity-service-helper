// <copyright file="CphUsersRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Cphs;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common;
using Defra.Identity.Repositories.Exceptions;
using Microsoft.Extensions.Logging;

public class CphUsersRepository(
    PostgresDbContext context,
    ReadOnlyPostgresDbContext readOnlyContext,
    ILogger<CphUsersRepository> logger) : ICphUsersRepository
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
        logger.LogInformation("Getting list of users for county parish holding");

        var primaryEntity = await readOnlyContext.CountyParishHoldings
            .FirstOrDefaultAsync(primaryPredicate, cancellationToken);

        if (primaryEntity == null)
        {
            throw new NotFoundException("County parish holding not found.");
        }

        var pagedResult = await readOnlyContext
            .Entry(primaryEntity)
            .Collection(p => p.ApplicationUserAccountHoldingAssignments)
            .Query()
            .Include(p => p.UserAccount)
            .Where(associationsPredicate)
            .ToPaged(pageNumber, pageSize, orderBy, orderByDescending, cancellationToken);

        return pagedResult;
    }
}

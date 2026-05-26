// <copyright file="CphAssignmentsRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Assignments;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common;
using Microsoft.Extensions.Logging;

public partial class CphAssignmentsRepository(
    ReadOnlyPostgresDbContext readOnlyContext,
    ILogger<CphAssignmentsRepository> logger)
    : ICphAssignmentsRepository
{
    public async Task<List<UserAccountCountyParishHoldingAssignments>> GetList(
        Expression<Func<UserAccountCountyParishHoldingAssignments, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting list of county parish holding assignments");

        var results = await GetQueryable(predicate)
            .ToListAsync(cancellationToken);

        return results;
    }

    public async Task<PagedEntities<UserAccountCountyParishHoldingAssignments>> GetPaged<TOrderBy>(
        Expression<Func<UserAccountCountyParishHoldingAssignments, bool>> predicate,
        int pageNumber,
        int pageSize,
        Expression<Func<UserAccountCountyParishHoldingAssignments, TOrderBy>> orderBy,
        bool orderByDescending,
        CancellationToken cancellationToken = default)
    {
        LogGettingListOfAssignmentsForCountyParishHolding();

        var results = await GetQueryable(predicate)
            .ToPaged(pageNumber, pageSize, orderBy, orderByDescending, cancellationToken);

        return results;
    }

    private IQueryable<UserAccountCountyParishHoldingAssignments> GetQueryable(Expression<Func<UserAccountCountyParishHoldingAssignments, bool>> predicate)
    {
        var results = readOnlyContext.UserAccountCountyParishHoldingAssignments
            .Include(p => p.CountyParishHolding)
            .Include(p => p.UserAccount)
            .Include(p => p.SpeciesType)
            .Include(p => p.Role)
            .Where(predicate);

        return results;
    }
}

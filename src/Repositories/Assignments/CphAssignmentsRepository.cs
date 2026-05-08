// <copyright file="CphAssignmentsRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Assignments;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common;
using Microsoft.Extensions.Logging;

public class CphAssignmentsRepository(
    ReadOnlyPostgresDbContext readOnlyContext,
    ILogger<CphAssignmentsRepository> logger) : ICphAssignmentsRepository
{
    public async Task<List<ApplicationUserAccountHoldingAssignments>> GetList(
        Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting list of county parish holding assignments");

        var results = await GetQueryable(predicate)
            .ToListAsync(cancellationToken);

        return results;
    }

    public async Task<PagedEntities<ApplicationUserAccountHoldingAssignments>> GetPaged<TOrderBy>(
        Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> predicate,
        int pageNumber,
        int pageSize,
        Expression<Func<ApplicationUserAccountHoldingAssignments, TOrderBy>> orderBy,
        bool orderByDescending,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting paged list of county parish holding assignments");

        var results = await GetQueryable(predicate)
            .ToPaged(pageNumber, pageSize, orderBy, orderByDescending, cancellationToken);

        return results;
    }

    private IQueryable<ApplicationUserAccountHoldingAssignments> GetQueryable(Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> predicate)
    {
        var results = readOnlyContext.ApplicationUserAccountHoldingAssignments
            .Include(p => p.CountyParishHolding)
            .Include(p => p.UserAccount)
            .Include(p => p.Application)
            .Include(p => p.Role)
            .Where(predicate);

        return results;
    }
}

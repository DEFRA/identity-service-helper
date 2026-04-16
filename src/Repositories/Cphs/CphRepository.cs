// <copyright file="CphRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Cphs;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common;
using Microsoft.Extensions.Logging;

public class CphRepository(
    PostgresDbContext context,
    ReadOnlyPostgresDbContext readOnlyContext,
    ILogger<CphRepository> logger)
    : ICphRepository
{
    public async Task<bool> ValidateReferenceById(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Validating county parish holding reference with id {Id}", id);

        var entity = await readOnlyContext.CountyParishHoldings
            .SingleOrDefaultAsync((entity) => entity.Id == id, cancellationToken);

        return entity is { DeletedAt: null } && (entity.ExpiredAt == null || DateTime.UtcNow < entity.ExpiredAt);
    }

    public async Task<CountyParishHoldings?> GetSingle(Expression<Func<CountyParishHoldings, bool>> predicate, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting single county parish holding");

        var result = await readOnlyContext.CountyParishHoldings
            .SingleOrDefaultAsync(predicate, cancellationToken);

        return result;
    }

    public async Task<PagedEntities<CountyParishHoldings>> GetPaged<TOrderBy>(
        Expression<Func<CountyParishHoldings, bool>> predicate,
        int pageNumber,
        int pageSize,
        Expression<Func<CountyParishHoldings, TOrderBy>> orderBy,
        bool orderByDescending,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting list of county parish holdings");

        var results = await readOnlyContext.CountyParishHoldings
            .Where(predicate)
            .ToPaged(pageNumber, pageSize, orderBy, orderByDescending, cancellationToken);

        return results;
    }

    public async Task<CountyParishHoldings> Update(CountyParishHoldings entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        logger.LogInformation("Updating county parish holding with id {Id}", entity.Id);

        context.Update(entity);

        await context.SaveChangesAsync(cancellationToken);

        return entity;
    }
}

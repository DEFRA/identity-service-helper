// <copyright file="CphRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Cphs;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common;
using Microsoft.Extensions.Logging;

public partial class CphRepository(
    PostgresDbContext context,
    ReadOnlyPostgresDbContext readOnlyContext,
    ILogger<CphRepository> logger)
    : ICphRepository
{
    public async Task<CountyParishHoldings?> GetSingle(
        Expression<Func<CountyParishHoldings, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        LogGettingSingleCountyParishHolding();

        var result = await readOnlyContext.CountyParishHoldings
            .Include(p => p.CountyParishHoldingAnimalSpecies)
            .ThenInclude(p => p.AnimalSpecies)
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
        LogGettingListOfCountyParishHoldings();

        var results = await readOnlyContext.CountyParishHoldings
            .Where(predicate)
            .Include(p => p.CountyParishHoldingAnimalSpecies)
            .ThenInclude(p => p.AnimalSpecies)
            .ToPaged(pageNumber, pageSize, orderBy, orderByDescending, cancellationToken);

        return results;
    }

    public async Task<CountyParishHoldings> Create(
        CountyParishHoldings entity,
        CancellationToken cancellationToken = default)
    {
        LogCreatingCountyParishHolding();

        var newCph = await context.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return newCph.Entity;
    }

    public async Task<CountyParishHoldings> Update(
        CountyParishHoldings entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        LogUpdatingCountyParishHoldingWithId(entity.Id);

        context.Update(entity);

        await context.SaveChangesAsync(cancellationToken);

        return entity;
    }
}

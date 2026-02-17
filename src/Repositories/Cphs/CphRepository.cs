// <copyright file="CphRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Cphs;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common;
using Defra.Identity.Repositories.Extensions;
using Microsoft.Extensions.Logging;

public class CphRepository(PostgresDbContext context, ILogger<ICphRepository> logger)
    : ICphRepository
{
    public async Task<CountyParishHoldings?> GetSingle(Expression<Func<CountyParishHoldings, bool>> predicate, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting single county parish holding");

        var result = await context.CountyParishHoldings
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

        var results = await context.CountyParishHoldings
            .Where(predicate)
            .ToPaged(pageNumber, pageSize, orderBy, orderByDescending, cancellationToken);

        return results;
    }

    public async Task<CountyParishHoldings> Create(CountyParishHoldings entity, Guid operatorId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<CountyParishHoldings> Update(CountyParishHoldings entity, Guid operatorId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<CountyParishHoldings?> Delete(Expression<Func<CountyParishHoldings, bool>> predicate, Guid operatorId, CancellationToken cancellationToken = default)
    {
        var cph = await context.CountyParishHoldings
            .SingleOrDefaultAsync(predicate, cancellationToken);

        if (cph == null)
        {
            return null;
        }

        logger.LogInformation("Deleting county parish holding with id {Id}", cph.Id);

        cph.DeletedById = operatorId;
        cph.DeletedAt = DateTime.UtcNow;

        context.CountyParishHoldings.Update(cph);

        await context.SaveChangesAsync(cancellationToken);

        return cph;
    }
}

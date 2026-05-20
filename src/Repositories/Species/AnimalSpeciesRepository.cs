// <copyright file="AnimalSpeciesRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Species;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Microsoft.Extensions.Logging;

public partial class AnimalSpeciesRepository(
    PostgresDbContext context,
    ReadOnlyPostgresDbContext readOnlyContext,
    ILogger<AnimalSpeciesRepository> logger)
    : IAnimalSpeciesRepository
{
    public async Task<AnimalSpecies?> GetSingle(
        Expression<Func<AnimalSpecies, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        LogGettingSingleAnimalSpecies();

        var query = await readOnlyContext.AnimalSpecies
            .SingleOrDefaultAsync(predicate, cancellationToken);

        return query;
    }

    public async Task<List<AnimalSpecies>> GetList(
        Expression<Func<AnimalSpecies, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        LogGettingListOfAnimalSpecies();

        var query = await readOnlyContext.AnimalSpecies
            .Where(predicate)
            .ToListAsync(cancellationToken);

        return query;
    }

    public async Task<AnimalSpecies> Update(
        AnimalSpecies entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        LogUpdatingAnimalSpeciesWithId(entity.Id);

        context.Update(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }
}

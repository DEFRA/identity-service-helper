// <copyright file="DataService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Ingest.CountyParishHoldings;

using Defra.Identity.KeeperReferenceData.Models.Sites;
using Defra.Identity.Repositories.Cphs;

public class DataService(ICphRepository cphRepository) : IDataService<Identifier>
{
    private const string Cphcode = "cphCode";

    public async Task Upsert(Identifier model, CancellationToken cancellationToken = default)
    {
         var existing = await cphRepository.GetSingle(x => x.Identifier.Equals(model.Value), cancellationToken);
         if (existing == null)
         {
             await cphRepository.Create(new Postgres.Database.Entities.CountyParishHoldings { Identifier = model.Value }, cancellationToken);
         }
    }
}

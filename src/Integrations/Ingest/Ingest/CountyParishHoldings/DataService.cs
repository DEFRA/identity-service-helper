// <copyright file="DataService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Ingest.CountyParishHoldings;

using Defra.Identity.KeeperReferenceData.Models.Sites;
using Defra.Identity.Repositories.Cphs;

public class DataService(ICphRepository cphRepository) : IDataService<Site>
{
    public Task Upsert(Site model, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

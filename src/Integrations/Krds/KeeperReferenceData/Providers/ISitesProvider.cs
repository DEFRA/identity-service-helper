// <copyright file="ISitesProvider.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Providers;

using Defra.Identity.KeeperReferenceData.Models;
using Defra.Identity.KeeperReferenceData.Models.Parties;

public interface ISitesProvider : IDisposable
{
    Task<List<Site>> Sites(DateTime since, CancellationToken cancellationToken);
}

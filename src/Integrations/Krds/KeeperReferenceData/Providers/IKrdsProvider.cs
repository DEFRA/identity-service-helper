// <copyright file="ISitesProviders.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Providers;

using Defra.Identity.KeeperReferenceData.Models;
using Defra.Identity.KeeperReferenceData.Models.Locations;
using Defra.Identity.KeeperReferenceData.Models.Parties;

public interface IKrdsProvider : IDisposable
{
    Task<SiteResponse> Sites(DateTime since, CancellationToken cancellationToken);

    Task<PartyResponse> Parties(DateTime since, CancellationToken cancellationToken);
}

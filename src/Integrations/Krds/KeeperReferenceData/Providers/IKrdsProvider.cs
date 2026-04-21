// <copyright file="IKrdsProvider.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

using Defra.Identity.KeeperReferenceData.Models.Parties;
using Defra.Identity.KeeperReferenceData.Models.Sites;

namespace Defra.Identity.KeeperReferenceData.Providers;

public interface IKrdsProvider : IDisposable
{
    Task<SiteResponse> Sites(DateTime since, CancellationToken cancellationToken);

    Task<PartyResponse> Parties(DateTime since, CancellationToken cancellationToken);
}

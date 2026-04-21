// <copyright file="IKrdsProvider.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Providers;

using Defra.Identity.KeeperReferenceData.Models.Parties;
using Defra.Identity.KeeperReferenceData.Models.Sites;

public interface IKrdsProvider : IDisposable
{
    Task<SiteResponse> Sites(DateTime since, CancellationToken cancellationToken);

    Task<PartyResponse> Parties(DateTime since, CancellationToken cancellationToken);
}

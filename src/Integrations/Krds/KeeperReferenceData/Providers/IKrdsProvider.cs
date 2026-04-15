// <copyright file="IKrdsProvider.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Providers;

using Defra.Identity.Models.Integration.Krds.Parties;
using Defra.Identity.Models.Integration.Krds.Sites;

public interface IKrdsProvider : IDisposable
{
    Task<SiteResponse> Sites(DateTime since, CancellationToken cancellationToken);

    Task<PartyResponse> Parties(DateTime since, CancellationToken cancellationToken);
}

// <copyright file="ISitesService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Services;

using Defra.Identity.KeeperReferenceData.Models;

public interface ISitesService
{
    Task<List<Site>> Sites(DateTime since, CancellationToken cancellationToken);
}

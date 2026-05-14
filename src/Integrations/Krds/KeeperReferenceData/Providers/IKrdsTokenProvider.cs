// <copyright file="IKrdsTokenProvider.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Providers;

/// <summary>
/// Provides access tokens for the KRDS API.
/// </summary>
public interface IKrdsTokenProvider
{
    /// <summary>
    /// Gets an access token.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the access token.</returns>
    Task<string> GetTokenAsync(CancellationToken cancellationToken = default);
}

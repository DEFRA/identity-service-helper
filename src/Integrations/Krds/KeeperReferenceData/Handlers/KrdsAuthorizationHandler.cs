// <copyright file="KrdsAuthorizationHandler.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Handlers;

using System.Net.Http.Headers;
using Defra.Identity.KeeperReferenceData.Providers;

/// <summary>
/// A message handler that adds a Bearer token to the request.
/// </summary>
public class KrdsAuthorizationHandler(IKrdsTokenProvider tokenProvider) : DelegatingHandler
{
    /// <inheritdoc/>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await tokenProvider.GetTokenAsync(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}

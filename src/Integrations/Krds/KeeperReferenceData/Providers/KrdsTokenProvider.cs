// <copyright file="KrdsTokenProvider.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Providers;

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Defra.Identity.KeeperReferenceData.Configuration;
using Microsoft.Extensions.Options;

/// <summary>
/// Provides access tokens for the KRDS API using OAuth2 client credentials grant.
/// </summary>
public class KrdsTokenProvider(HttpClient httpClient, IOptions<KrdsApi> options) : IKrdsTokenProvider
{
    private readonly KrdsApi krdsApi = options.Value;
    private string? cachedToken;
    private DateTime tokenExpiry = DateTime.MinValue;

    /// <inheritdoc/>
    public async Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(this.cachedToken) && this.tokenExpiry > DateTime.UtcNow.AddMinutes(1))
        {
            return this.cachedToken;
        }

        var request = new HttpRequestMessage(HttpMethod.Post, this.krdsApi.TokenUrl);
        var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{this.krdsApi.ClientId}:{this.krdsApi.ClientSecret}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
        });

        var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content);

        if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
        {
            throw new InvalidOperationException("Failed to retrieve access token from KRDS API.");
        }

        this.cachedToken = tokenResponse.AccessToken;
        this.tokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);

        return this.cachedToken;
    }

    private class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }
    }
}

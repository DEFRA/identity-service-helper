// <copyright file="KrdsTokenProviderTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Tests;

using System.Net;
using System.Text;
using System.Text.Json;
using Defra.Identity.KeeperReferenceData.Configuration;
using Defra.Identity.KeeperReferenceData.Providers;
using Microsoft.Extensions.Options;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

public class KrdsTokenProviderTests
{
    [Fact]
    public async Task GetTokenAsync_ReturnsToken_And_CachesIt()
    {
        using var server = WireMockServer.Start();
        var tokenUrl = $"{server.Url}/oauth2/token";
        var clientId = "test-client-id";
        var clientSecret = "test-client-secret";
        var expectedToken = "test-access-token";

        server
            .Given(Request.Create()
                .WithPath("/oauth2/token")
                .WithHeader("Authorization", "Basic dGVzdC1jbGllbnQtaWQ6dGVzdC1jbGllbnQtc2VjcmV0")
                .WithBody("grant_type=client_credentials")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(new
                {
                    access_token = expectedToken,
                    expires_in = 3600,
                    token_type = "Bearer"
                })));

        var options = Options.Create(new KrdsApi
        {
            TokenUrl = tokenUrl,
            ClientId = clientId,
            ClientSecret = clientSecret
        });

        var httpClient = new HttpClient();
        var sut = new KrdsTokenProvider(httpClient, options);

        // Act
        var token1 = await sut.GetTokenAsync();
        var token2 = await sut.GetTokenAsync();

        // Assert
        Assert.Equal(expectedToken, token1);
        Assert.Equal(expectedToken, token2);

        // Verify only one call was made to the server (caching works)
        Assert.Equal(1, server.LogEntries.Count(le => le.RequestMessage.Path == "/oauth2/token"));
    }

    [Fact]
    public async Task GetTokenAsync_ThrowsException_OnFailure()
    {
        using var server = WireMockServer.Start();
        var tokenUrl = $"{server.Url}/oauth2/token";

        server
            .Given(Request.Create().WithPath("/oauth2/token").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(401));

        var options = Options.Create(new KrdsApi
        {
            TokenUrl = tokenUrl,
            ClientId = "id",
            ClientSecret = "secret"
        });

        var httpClient = new HttpClient();
        var sut = new KrdsTokenProvider(httpClient, options);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => sut.GetTokenAsync());
    }
}

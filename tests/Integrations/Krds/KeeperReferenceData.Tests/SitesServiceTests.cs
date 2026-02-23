// <copyright file="SitesServiceTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Tests;

using System;
using System.Threading;
using System.Threading.Tasks;
using Defra.Identity.KeeperReferenceData.Configuration;
using Defra.Identity.KeeperReferenceData.Models;
using Defra.Identity.KeeperReferenceData.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

public class SitesServiceTests
{
    [Fact]
    public async Task Sites_Returns_List_And_Calls_Krds_With_Header_And_Since_Query()
    {
        // Arrange
        using var server = WireMockServer.Start();

        var api = Options.Create(new KrdsApi
        {
            Url = server.Urls[0],
            Key = "secret-key"
        });
        var logger = NullLogger<SitesService>.Instance;
        var sut = new SitesService(logger, api);

        var since = new DateTime(2024, 12, 25, 10, 30, 0, DateTimeKind.Utc);
        var expectedSince = since.ToString("yyyy-MM-ddTHH:mm:ssZ");

        server
            .Given(Request.Create().UsingGet().WithPath("/sites").WithHeader("x-api-key", "secret-key").WithParam("since", expectedSince))
            .RespondWith(Response.Create().WithStatusCode(200).WithHeader("Content-Type", "application/json").WithBodyAsJson(new[]
            {
                new Site { Id = "1" },
                new Site { Id = "2" }
            }));

        // Act
        var result = await sut.Sites(since, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        // Verify request to KRDS API
        Assert.Single(server.LogEntries);
        var entry = server.LogEntries[0];
        Assert.Equal("/sites", entry.RequestMessage.Path);
        Assert.True(entry.RequestMessage.Headers.ContainsKey("x-api-key"));
        Assert.Contains("secret-key", entry.RequestMessage.Headers["x-api-key"]);
        Assert.True(entry.RequestMessage.Query.ContainsKey("since"));
        Assert.Equal(expectedSince, entry.RequestMessage.Query["since"]);
    }

    [Fact]
    public async Task Sites_Rethrows_On_FlurlHttpException()
    {
        // Arrange
        using var server = WireMockServer.Start();

        var api = Options.Create(new KrdsApi
        {
            Url = server.Urls[0],
            Key = "secret-key"
        });
        var logger = NullLogger<SitesService>.Instance;
        var sut = new SitesService(logger, api);

        server
            .Given(Request.Create().UsingGet().WithPath("/sites").WithHeader("x-api-key", "secret-key"))
            .RespondWith(Response.Create().WithStatusCode(500));

        // Act & Assert
        await Assert.ThrowsAsync<Flurl.Http.FlurlHttpException>(() => sut.Sites(DateTime.UtcNow, CancellationToken.None));

        Assert.Single(server.LogEntries);
        var entry = server.LogEntries[0];
        Assert.Equal("/sites", entry.RequestMessage.Path);
        Assert.True(entry.RequestMessage.Headers.ContainsKey("x-api-key"));
        Assert.Contains("secret-key", entry.RequestMessage.Headers["x-api-key"]);
    }
}

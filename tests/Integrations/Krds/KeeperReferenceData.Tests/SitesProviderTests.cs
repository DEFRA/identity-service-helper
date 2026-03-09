// <copyright file="SitesProviderTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Tests;

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Defra.Identity.KeeperReferenceData.Models;
using Defra.Identity.KeeperReferenceData.Providers;
using Microsoft.Extensions.Logging.Abstractions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

public class SitesProviderTests
{
    [Fact]
    public async Task Sites_Returns_List_When_Response_Is_Json_Array()
    {
        using var server = WireMockServer.Start();
        server
            .Given(Request.Create().WithPath("/sites").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("[{},{},{}]"));

        var httpClient = new HttpClient { BaseAddress = new Uri(server.Url + "/") };
        var logger = NullLogger<SitesProvider>.Instance;
        using var sut = new SitesProvider(httpClient, logger);

        var result = await sut.Sites(DateTime.UtcNow, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<List<Site>>(result);
        Assert.Equal(3, result.Count);

        // Verify the request path was called at least once
        Assert.True(server.LogEntries.Count(le => le.RequestMessage.Path == "/sites" && le.RequestMessage.Method == "GET") >= 1);
    }

    [Fact]
    public async Task Sites_Returns_Empty_List_When_Response_Body_Is_Empty()
    {
        using var server = WireMockServer.Start();
        server
            .Given(Request.Create().WithPath("/sites").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(string.Empty));

        var httpClient = new HttpClient { BaseAddress = new Uri(server.Url + "/") };
        var logger = NullLogger<SitesProvider>.Instance;
        using var sut = new SitesProvider(httpClient, logger);

        var result = await sut.Sites(DateTime.UtcNow, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
        Assert.True(server.LogEntries.Count(le => le.RequestMessage.Path == "/sites" && le.RequestMessage.Method == "GET") >= 1);
    }

    [Fact]
    public async Task Sites_Throws_On_NonSuccess_StatusCode()
    {
        using var server = WireMockServer.Start();
        server
            .Given(Request.Create().WithPath("/sites").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode((int)HttpStatusCode.InternalServerError));

        var httpClient = new HttpClient { BaseAddress = new Uri(server.Url + "/") };
        var logger = NullLogger<SitesProvider>.Instance;
        using var sut = new SitesProvider(httpClient, logger);

        await Assert.ThrowsAsync<HttpRequestException>(() => sut.Sites(DateTime.UtcNow, CancellationToken.None));
        Assert.True(server.LogEntries.Count(le => le.RequestMessage.Path == "/sites" && le.RequestMessage.Method == "GET") >= 1);
    }
}

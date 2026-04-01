// <copyright file="SitesProviderTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Tests;

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Defra.Identity.KeeperReferenceData.Providers;
using Defra.Identity.Models.Integration.Krds.Locations;
using Defra.Identity.Models.Integration.Krds.Parties;
using Microsoft.Extensions.Logging.Abstractions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

public class KrdsProviderTests
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
        var logger = NullLogger<KrdsProvider>.Instance;
        using var sut = new KrdsProvider(httpClient, logger);

        var result = await sut.Sites(DateTime.UtcNow, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<SiteResponse>(result);
        Assert.Equal(3, result.Count);

        // Verify the request path was called at least once
        Assert.True(server.LogEntries.Count(le => le.RequestMessage.Path == "/sites" && le.RequestMessage.Method == "GET") >= 1);
    }

    [Fact]
    public async Task Sites_Returns_ResponseObject_When_Response_Is_Json_Object()
    {
        using var server = WireMockServer.Start();
        server
            .Given(Request.Create().WithPath("/sites").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"count\": 2, \"values\": [{}, {}]}"));

        var httpClient = new HttpClient { BaseAddress = new Uri(server.Url + "/") };
        var logger = NullLogger<KrdsProvider>.Instance;
        using var sut = new KrdsProvider(httpClient, logger);

        var result = await sut.Sites(DateTime.UtcNow, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<SiteResponse>(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(2, result.Values.Count);
    }

    [Fact]
    public async Task Parties_Returns_List_When_Response_Is_Json_Array()
    {
        using var server = WireMockServer.Start();
        server
            .Given(Request.Create().WithPath("/parties").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("[{},{},{}]"));

        var httpClient = new HttpClient { BaseAddress = new Uri(server.Url + "/") };
        var logger = NullLogger<KrdsProvider>.Instance;
        using var sut = new KrdsProvider(httpClient, logger);

        var result = await sut.Parties(DateTime.UtcNow, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<PartyResponse>(result);
        Assert.Equal(3, result.Count);

        // Verify the request path was called at least once
        Assert.True(server.LogEntries.Count(le => le.RequestMessage.Path == "/parties" && le.RequestMessage.Method == "GET") >= 1);
    }

    [Fact]
    public async Task Sites_Returns_Empty_List_When_Response_Body_Is_Empty()
    {
        using var server = WireMockServer.Start();
        server
            .Given(Request.Create().WithPath("/sites").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", MediaTypeNames.Application.Json)
                .WithBody(string.Empty));

        var httpClient = new HttpClient { BaseAddress = new Uri(server.Url + "/") };
        var logger = NullLogger<KrdsProvider>.Instance;
        using var sut = new KrdsProvider(httpClient, logger);

        var result = await sut.Sites(DateTime.UtcNow, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.Values);
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
        var logger = NullLogger<KrdsProvider>.Instance;
        using var sut = new KrdsProvider(httpClient, logger);

        await Assert.ThrowsAsync<HttpRequestException>(() => sut.Sites(DateTime.UtcNow, CancellationToken.None));
        Assert.True(server.LogEntries.Count(le => le.RequestMessage.Path == "/sites" && le.RequestMessage.Method == "GET") >= 1);
    }
}

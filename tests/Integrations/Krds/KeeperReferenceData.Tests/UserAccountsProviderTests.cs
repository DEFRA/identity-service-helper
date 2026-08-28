// <copyright file="UserAccountsProviderTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Tests;

using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Defra.Identity.KeeperReferenceData.Models.UserAccounts;
using Defra.Identity.KeeperReferenceData.Providers;
using Microsoft.Extensions.Logging.Abstractions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

public class UserAccountsProviderTests
{
    private const string AccountBody = """
        {
          "id": "1c2a1b46-1f2f-4f2f-9b3a-4f0e6a1d4c11",
          "subject": "9f3a1c2e-0b6d-4f4e-9d2a-7c8b1e5f0a3d",
          "email": "jane.farmer@example.com",
          "firstName": "Jane",
          "lastName": "Farmer",
          "displayName": "Jane Farmer",
          "associationsRefreshedDate": "2026-08-20T06:00:00Z",
          "cphAssociations": [
            {
              "id": "cph-identifier-1",
              "cphNumber": "22/001/0001",
              "role": "owner",
              "partyId": "party-1",
              "holdingId": "holding-1",
              "holdingName": "Hill Farm"
            }
          ]
        }
        """;

    private static readonly EnsureUserAccountRequest Claims = new()
    {
        Sub = "9f3a1c2e-0b6d-4f4e-9d2a-7c8b1e5f0a3d",
        Email = "jane.farmer@example.com",
        GivenName = "Jane",
        FamilyName = "Farmer",
    };

    [Fact]
    public async Task EnsureUserAccount_Returns_Created_When_Krds_Creates_The_Account()
    {
        using var server = WireMockServer.Start();
        server
            .Given(Request.Create().WithPath("/v2/user-accounts").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(201)
                .WithHeader("Content-Type", "application/json")
                .WithBody(AccountBody));

        using var sut = CreateProvider(server);

        var result = await sut.EnsureUserAccount(Claims, CancellationToken.None);

        Assert.True(result.Created);
        Assert.Equal("jane.farmer@example.com", result.Account.Email);
        Assert.Equal("Jane Farmer", result.Account.DisplayName);
        Assert.Equal("22/001/0001", Assert.Single(result.Account.CphAssociations).CphNumber);
    }

    [Fact]
    public async Task EnsureUserAccount_Returns_Not_Created_When_Krds_Returns_Ok()
    {
        using var server = WireMockServer.Start();
        server
            .Given(Request.Create().WithPath("/v2/user-accounts").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(AccountBody));

        using var sut = CreateProvider(server);

        var result = await sut.EnsureUserAccount(Claims, CancellationToken.None);

        Assert.False(result.Created);
        Assert.True(server.LogEntries.Count(le => le.RequestMessage is { Path: "/v2/user-accounts", Method: "POST" }) >= 1);
    }

    [Fact]
    public async Task EnsureUserAccount_Throws_When_Krds_Fails()
    {
        using var server = WireMockServer.Start();
        server
            .Given(Request.Create().WithPath("/v2/user-accounts").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(500));

        using var sut = CreateProvider(server);

        await Assert.ThrowsAsync<HttpRequestException>(
            () => sut.EnsureUserAccount(Claims, CancellationToken.None));
    }

    [Fact]
    public async Task GetUserAccountBySubject_Returns_Account()
    {
        using var server = WireMockServer.Start();
        server
            .Given(Request.Create()
                .WithPath("/v2/user-accounts/9f3a1c2e-0b6d-4f4e-9d2a-7c8b1e5f0a3d")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(AccountBody));

        using var sut = CreateProvider(server);

        var result = await sut.GetUserAccountBySubject(
            "9f3a1c2e-0b6d-4f4e-9d2a-7c8b1e5f0a3d",
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("9f3a1c2e-0b6d-4f4e-9d2a-7c8b1e5f0a3d", result.Subject);
    }

    [Fact]
    public async Task GetUserAccountBySubject_Returns_Null_When_Not_Found()
    {
        using var server = WireMockServer.Start();
        server
            .Given(Request.Create().WithPath("/v2/user-accounts/unknown").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(404));

        using var sut = CreateProvider(server);

        var result = await sut.GetUserAccountBySubject("unknown", CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserAccountBySubject_Percent_Encodes_The_Subject()
    {
        using var server = WireMockServer.Start();
        server
            .Given(Request.Create().WithPath("/v2/user-accounts/a b").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(AccountBody));

        using var sut = CreateProvider(server);

        var result = await sut.GetUserAccountBySubject("a b", CancellationToken.None);

        Assert.NotNull(result);
    }

    private static UserAccountsProvider CreateProvider(WireMockServer server)
    {
        var httpClient = new HttpClient { BaseAddress = new Uri(server.Url + "/") };

        return new UserAccountsProvider(httpClient, NullLogger<UserAccountsProvider>.Instance);
    }
}

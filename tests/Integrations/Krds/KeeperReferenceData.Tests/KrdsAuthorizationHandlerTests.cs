// <copyright file="KrdsAuthorizationHandlerTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Tests;

using System.Net;
using System.Net.Http.Headers;
using Defra.Identity.KeeperReferenceData.Handlers;
using Defra.Identity.KeeperReferenceData.Providers;
using NSubstitute;
using Xunit;

public class KrdsAuthorizationHandlerTests
{
    [Fact]
    public async Task SendAsync_Adds_Authorization_Header()
    {
        // Arrange
        var token = "test-token";
        var mockTokenProvider = Substitute.For<IKrdsTokenProvider>();
        mockTokenProvider
            .GetTokenAsync(Arg.Any<CancellationToken>())
            .Returns(token);

        var handler = new KrdsAuthorizationHandler(mockTokenProvider)
        {
            InnerHandler = new TestHandler(),
        };

        var invoker = new HttpMessageInvoker(handler);
        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/test");

        // Act
        var response = await invoker.SendAsync(request, CancellationToken.None);

        // Assert
        response.ShouldSatisfyAllConditions(
            x => x.StatusCode.ShouldBe(HttpStatusCode.OK));
        request.ShouldSatisfyAllConditions(
            x => x.Headers.Authorization?.Scheme.ShouldBe("Bearer"),
            x => x.Headers.Authorization?.Parameter.ShouldBe("test-token"),
            x => x.Headers.Authorization?.Parameter.ShouldNotBeNull(),
            x => x.Headers.Authorization?.Parameter.ShouldBeEquivalentTo(token));

        await mockTokenProvider.Received(1).GetTokenAsync(Arg.Any<CancellationToken>());
    }

    private class TestHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}

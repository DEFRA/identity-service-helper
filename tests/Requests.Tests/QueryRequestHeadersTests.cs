// <copyright file="QueryRequestHeadersTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Tests;

using Microsoft.AspNetCore.Http;
using Shouldly;
using Xunit;

public class QueryRequestHeadersTests
{
    [Fact]
    public void TryGet_Returns_False_When_Item_Not_Present()
    {
        var context = new DefaultHttpContext();
        var result = QueryRequestHeaders.TryGet(context, out var headers);
        result.ShouldBeFalse();
        headers.ShouldBeNull();
    }

    [Fact]
    public void TryGet_Returns_True_When_Item_Is_Present()
    {
        var context = new DefaultHttpContext();
        var expectedHeaders = new QueryRequestHeaders(Guid.NewGuid(), "api-key");
        context.Items[QueryRequestHeaders.ItemKey] = expectedHeaders;

        var result = QueryRequestHeaders.TryGet(context, out var headers);
        result.ShouldBeTrue();
        headers.ShouldBe(expectedHeaders);
    }

    [Fact]
    public void TryGet_Returns_False_When_Item_Is_Wrong_Type()
    {
        var context = new DefaultHttpContext();
        context.Items[QueryRequestHeaders.ItemKey] = "not-the-right-type";

        var result = QueryRequestHeaders.TryGet(context, out var headers);
        result.ShouldBeFalse();
        headers.ShouldBeNull();
    }

    [Fact]
    public async Task BindAsync_Returns_Existing_From_Context_Items()
    {
        var context = new DefaultHttpContext();
        var expectedHeaders = new QueryRequestHeaders(Guid.NewGuid(), "api-key");
        context.Items[QueryRequestHeaders.ItemKey] = expectedHeaders;

        var result = await QueryRequestHeaders.BindAsync(context, null!);
        result.ShouldBe(expectedHeaders);
    }

    [Fact]
    public async Task BindAsync_Binds_From_Headers_When_Not_In_Context_Items()
    {
        var context = new DefaultHttpContext();
        var correlationId = Guid.NewGuid();
        context.Request.Headers[IdentityHeaderNames.CorrelationId] = correlationId.ToString();
        context.Request.Headers[IdentityHeaderNames.ApiKey] = "api-key";

        var result = await QueryRequestHeaders.BindAsync(context, null!);

        result.ShouldSatisfyAllConditions(
            x => x.ApiKey.ShouldBeOfType<string>(),
            x => x.ApiKey.ShouldBe("api-key"),
            x => x.CorrelationId.ShouldBeOfType<Guid>(),
            x => x.CorrelationId.ShouldBe(correlationId));
    }

    [Fact]
    public async Task BindAsync_Throws_When_CorrelationId_Missing()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[IdentityHeaderNames.ApiKey] = "api-key";

        var exception = await Should.ThrowAsync<BadHttpRequestException>(async () => await QueryRequestHeaders.BindAsync(context, null!));

        exception.ShouldSatisfyAllConditions(
            x => x.ShouldBeOfType<BadHttpRequestException>(),
            x => x.StatusCode.ShouldBe(StatusCodes.Status400BadRequest),
            x => x.Message.ShouldBe($"Header {IdentityHeaderNames.CorrelationId} is required."));
    }

    [Fact]
    public async Task BindAsync_Throws_When_CorrelationId_Whitespace()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[IdentityHeaderNames.CorrelationId] = "   ";
        context.Request.Headers[IdentityHeaderNames.ApiKey] = "api-key";

        var exception = await Should.ThrowAsync<BadHttpRequestException>(async () => await QueryRequestHeaders.BindAsync(context, null!));
        exception.ShouldSatisfyAllConditions(
            x => x.ShouldBeOfType<BadHttpRequestException>(),
            x => x.StatusCode.ShouldBe(StatusCodes.Status400BadRequest),
            x => x.Message.ShouldBe($"Header {IdentityHeaderNames.CorrelationId} is required."));
    }

    [Fact]
    public async Task BindAsync_Throws_When_ApiKey_Missing()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[IdentityHeaderNames.CorrelationId] = Guid.NewGuid().ToString();

        var exception = await Should.ThrowAsync<BadHttpRequestException>(async () => await QueryRequestHeaders.BindAsync(context, null!));
        exception.ShouldSatisfyAllConditions(
            x => x.ShouldBeOfType<BadHttpRequestException>(),
            x => x.StatusCode.ShouldBe(StatusCodes.Status400BadRequest),
            x => x.Message.ShouldBe($"Header {IdentityHeaderNames.ApiKey} is required."));
    }

    [Fact]
    public async Task BindAsync_Throws_When_ApiKey_Whitespace()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[IdentityHeaderNames.CorrelationId] = Guid.NewGuid().ToString();
        context.Request.Headers[IdentityHeaderNames.ApiKey] = "   ";

        var exception = await Should.ThrowAsync<BadHttpRequestException>(async () => await QueryRequestHeaders.BindAsync(context, null!));
        exception.ShouldSatisfyAllConditions(
            x => x.ShouldBeOfType<BadHttpRequestException>(),
            x => x.StatusCode.ShouldBe(StatusCodes.Status400BadRequest),
            x => x.Message.ShouldBe($"Header {IdentityHeaderNames.ApiKey} is required."));
    }
}

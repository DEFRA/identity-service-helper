// <copyright file="IdentityRequestHeadersTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Unit.Tests.Requests;

using Defra.Identity.Requests;
using Microsoft.AspNetCore.Http;
using Shouldly;
using Xunit;

public class IdentityRequestHeadersTests
{
    [Fact]
    public void TryGet_Returns_False_When_Item_Not_Present()
    {
        var context = new DefaultHttpContext();
        var result = IdentityRequestHeaders.TryGet(context, out var headers);
        result.ShouldBeFalse();
        headers.ShouldBeNull();
    }

    [Fact]
    public void TryGet_Returns_True_When_Item_Is_Present()
    {
        var context = new DefaultHttpContext();
        var expectedHeaders = new IdentityRequestHeaders(Guid.NewGuid(), Guid.NewGuid(), "api-key");
        context.Items[IdentityRequestHeaders.ItemKey] = expectedHeaders;

        var result = IdentityRequestHeaders.TryGet(context, out var headers);
        result.ShouldBeTrue();
        headers.ShouldBe(expectedHeaders);
    }

    [Fact]
    public void TryGet_Returns_False_When_Item_Is_Wrong_Type()
    {
        var context = new DefaultHttpContext();
        context.Items[IdentityRequestHeaders.ItemKey] = "not-the-right-type";

        var result = IdentityRequestHeaders.TryGet(context, out var headers);
        result.ShouldBeFalse();
        headers.ShouldBeNull();
    }

    [Fact]
    public async Task BindAsync_Returns_Existing_From_Context_Items()
    {
        var context = new DefaultHttpContext();
        var expectedHeaders = new IdentityRequestHeaders(Guid.NewGuid(), Guid.NewGuid(), "api-key");
        context.Items[IdentityRequestHeaders.ItemKey] = expectedHeaders;

        var result = await IdentityRequestHeaders.BindAsync(context, null!);
        result.ShouldBe(expectedHeaders);
    }

    [Fact]
    public async Task BindAsync_Binds_From_Headers_When_Not_In_Context_Items()
    {
        var context = new DefaultHttpContext();
        var correlationId = Guid.NewGuid();
        var operatorId = Guid.NewGuid();
        context.Request.Headers[IdentityHeaderNames.CorrelationId] = correlationId.ToString();
        context.Request.Headers[IdentityHeaderNames.OperatorId] = operatorId.ToString();
        context.Request.Headers[IdentityHeaderNames.ApiKey] = "api-key";

        var result = await IdentityRequestHeaders.BindAsync(context, null!);

        result.ShouldSatisfyAllConditions(
            x => x.OperatorId.ShouldBeOfType<Guid>(),
            x => x.OperatorId.ShouldBe(operatorId),
            x => x.ApiKey.ShouldBeOfType<string>(),
            x => x.ApiKey.ShouldBe("api-key"),
            x => x.CorrelationId.ShouldBeOfType<Guid>(),
            x => x.CorrelationId.ShouldBe(correlationId));
    }

    [Fact]
    public async Task BindAsync_Throws_When_CorrelationId_Missing()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[IdentityHeaderNames.OperatorId] = "op-id";
        context.Request.Headers[IdentityHeaderNames.ApiKey] = "api-key";

        var exception = await Should.ThrowAsync<BadHttpRequestException>(async () => await IdentityRequestHeaders.BindAsync(context, null!));
        exception.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        exception.Message.ShouldBe($"Header {IdentityHeaderNames.CorrelationId} is required.");
    }

    [Fact]
    public async Task BindAsync_Throws_When_CorrelationId_Whitespace()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[IdentityHeaderNames.CorrelationId] = "   ";
        context.Request.Headers[IdentityHeaderNames.OperatorId] = "op-id";
        context.Request.Headers[IdentityHeaderNames.ApiKey] = "api-key";

        var exception = await Should.ThrowAsync<BadHttpRequestException>(async () => await IdentityRequestHeaders.BindAsync(context, null!));
        exception.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        exception.Message.ShouldBe($"Header {IdentityHeaderNames.CorrelationId} is required.");
    }

    [Fact]
    public async Task BindAsync_Throws_When_OperatorId_Missing()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[IdentityHeaderNames.CorrelationId] = "corr-id";
        context.Request.Headers[IdentityHeaderNames.ApiKey] = "api-key";

        var exception = await Should.ThrowAsync<BadHttpRequestException>(async () => await IdentityRequestHeaders.BindAsync(context, null!));
        exception.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        exception.Message.ShouldBe($"Header {IdentityHeaderNames.OperatorId} is required.");
    }

    [Fact]
    public async Task BindAsync_Throws_When_OperatorId_Whitespace()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[IdentityHeaderNames.CorrelationId] = "corr-id";
        context.Request.Headers[IdentityHeaderNames.OperatorId] = "   ";
        context.Request.Headers[IdentityHeaderNames.ApiKey] = "api-key";

        var exception = await Should.ThrowAsync<BadHttpRequestException>(async () => await IdentityRequestHeaders.BindAsync(context, null!));
        exception.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        exception.Message.ShouldBe($"Header {IdentityHeaderNames.OperatorId} is required.");
    }

    [Fact]
    public async Task BindAsync_Throws_When_ApiKey_Missing()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[IdentityHeaderNames.CorrelationId] = "corr-id";
        context.Request.Headers[IdentityHeaderNames.OperatorId] = "op-id";

        var exception = await Should.ThrowAsync<BadHttpRequestException>(async () => await IdentityRequestHeaders.BindAsync(context, null!));
        exception.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        exception.Message.ShouldBe($"Header {IdentityHeaderNames.ApiKey} is required.");
    }

    [Fact]
    public async Task BindAsync_Throws_When_ApiKey_Whitespace()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[IdentityHeaderNames.CorrelationId] = "corr-id";
        context.Request.Headers[IdentityHeaderNames.OperatorId] = "op-id";
        context.Request.Headers[IdentityHeaderNames.ApiKey] = "   ";

        var exception = await Should.ThrowAsync<BadHttpRequestException>(async () => await IdentityRequestHeaders.BindAsync(context, null!));
        exception.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        exception.Message.ShouldBe($"Header {IdentityHeaderNames.ApiKey} is required.");
    }
}

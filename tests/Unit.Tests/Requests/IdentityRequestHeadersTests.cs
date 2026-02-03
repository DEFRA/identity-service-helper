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
        var expectedHeaders = new IdentityRequestHeaders("corr-id", "op-id", "api-key");
        context.Items[IdentityRequestHeaders.ItemKey] = expectedHeaders;

        var result = IdentityRequestHeaders.TryGet(context, out var headers);
        result.ShouldBeTrue();
        headers.ShouldBe(expectedHeaders);
    }

    [Fact]
    public async Task BindAsync_Returns_Existing_From_Context_Items()
    {
        var context = new DefaultHttpContext();
        var expectedHeaders = new IdentityRequestHeaders("corr-id", "op-id", "api-key");
        context.Items[IdentityRequestHeaders.ItemKey] = expectedHeaders;

        var result = await IdentityRequestHeaders.BindAsync(context, null!);
        result.ShouldBe(expectedHeaders);
    }

    [Fact]
    public async Task BindAsync_Binds_From_Headers_When_Not_In_Context_Items()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[IdentityHeaderNames.CorrelationId] = "corr-id";
        context.Request.Headers[IdentityHeaderNames.OperatorId] = "op-id";
        context.Request.Headers[IdentityHeaderNames.ApiKey] = "api-key";

        var result = await IdentityRequestHeaders.BindAsync(context, null!);
        result.CorrelationId.ShouldBe("corr-id");
        result.OperatorId.ShouldBe("op-id");
        result.ApiKey.ShouldBe("api-key");
    }

    [Fact]
    public async Task BindAsync_Throws_When_CorrelationId_Missing()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[IdentityHeaderNames.OperatorId] = "op-id";
        context.Request.Headers[IdentityHeaderNames.ApiKey] = "api-key";

        await Should.ThrowAsync<BadHttpRequestException>(async () => await IdentityRequestHeaders.BindAsync(context, null!));
    }

    [Fact]
    public async Task BindAsync_Throws_When_OperatorId_Missing()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[IdentityHeaderNames.CorrelationId] = "corr-id";
        context.Request.Headers[IdentityHeaderNames.ApiKey] = "api-key";

        await Should.ThrowAsync<BadHttpRequestException>(async () => await IdentityRequestHeaders.BindAsync(context, null!));
    }

    [Fact]
    public async Task BindAsync_Throws_When_ApiKey_Missing()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[IdentityHeaderNames.CorrelationId] = "corr-id";
        context.Request.Headers[IdentityHeaderNames.OperatorId] = "op-id";

        await Should.ThrowAsync<BadHttpRequestException>(async () => await IdentityRequestHeaders.BindAsync(context, null!));
    }
}

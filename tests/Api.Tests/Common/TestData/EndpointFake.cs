// <copyright file="EndpointFake.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace Defra.Identity.Api.Tests.Common.TestData;

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;

[ExcludeFromCodeCoverage]
public class EndpointFake<TRequest>
{
    private readonly IResult result;

    private EndpointFake(IResult result)
    {
        this.result = result;
    }

    public TRequest? CapturedRequest { get; private set; }

    public ServiceFake? CapturedService { get; private set; }

    public int CapturedCallCount { get; private set; } = 0;

    public static EndpointFake<TRequest> Create(IResult result)
    {
        return new EndpointFake<TRequest>(result);
    }

    public async Task<IResult> FakeHandlerMethod(
        [AsParameters] TRequest request,
        ServiceFake service)
    {
        this.CapturedRequest = request;
        this.CapturedService = service;
        this.CapturedCallCount += 1;

        return this.result;
    }
}

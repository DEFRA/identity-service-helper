// <copyright file="CphEndpointFake.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace Defra.Identity.Api.Tests.Endpoints.Cphs.Handlers.TestData;

using Defra.Identity.Services.Cphs;
using Microsoft.AspNetCore.Http;

public class CphEndpointFake<TRequest>
{
    private readonly IResult result;

    private CphEndpointFake(IResult result)
    {
        this.result = result;
    }

    public HeadersFake? CapturedHeaders { get; private set; }

    public TRequest? CapturedRequest { get; private set; }

    public ICphService? CapturedCphService { get; private set; }

    public int CapturedCallCount { get; private set; } = 0;

    public static CphEndpointFake<TRequest> Create(IResult result)
    {
        return new CphEndpointFake<TRequest>(result);
    }

    public async Task<IResult> FakeHandlerMethod(
        HeadersFake headers,
        [AsParameters] TRequest request,
        ICphService service)
    {
        this.CapturedHeaders = headers;
        this.CapturedRequest = request;
        this.CapturedCphService = service;
        this.CapturedCallCount += 1;

        return this.result;
    }
}

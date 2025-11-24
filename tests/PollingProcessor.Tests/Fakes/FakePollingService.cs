// <copyright file="FakePollingService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.PollingProcessor.Tests.Fakes;

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

[ExcludeFromCodeCoverage]
public class FakePollingService(
    ILogger<FakePollingService> logger,
    IOptions<Fakes.FakePollingConfiguration> options)
    : IFakePollingService
{
    public Task Execute(IJobExecutionContext context)
    {
        return Task.CompletedTask;
    }
}

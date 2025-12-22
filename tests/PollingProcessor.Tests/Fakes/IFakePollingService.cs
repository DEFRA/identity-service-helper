// <copyright file="IFakePollingService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.PollingProcessor.Tests.Fakes;

using Quartz;

public interface IFakePollingService
    : IJob
{
}

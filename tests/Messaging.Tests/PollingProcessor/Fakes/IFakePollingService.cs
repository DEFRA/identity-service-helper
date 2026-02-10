// <copyright file="IFakePollingService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Messaging.Tests.PollingProcessor.Fakes;

using Quartz;

public interface IFakePollingService
    : IJob
{
}

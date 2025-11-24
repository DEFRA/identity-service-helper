// <copyright file="IFakePollingService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.PollingProcessor.Tests.Fakes;

using System.Diagnostics.CodeAnalysis;
using Quartz;

public interface IFakePollingService
    : IJob
{
}

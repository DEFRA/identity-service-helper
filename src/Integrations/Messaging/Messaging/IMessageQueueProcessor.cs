// <copyright file="IMessageQueueProcessor.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Messaging;

using Defra.Identity.Messaging.Models;

public interface IMessageQueueProcessor
{
    Task<ProcessResult> ProcessMessageQueueAsync(CancellationToken cancellationToken = default);
}

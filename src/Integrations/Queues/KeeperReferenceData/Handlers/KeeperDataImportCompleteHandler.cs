// <copyright file="KeeperDataImportCompleteHandler.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Handlers;

using AWS.Messaging;
using Defra.Identity.KeeperReferenceData.Messages;
using Microsoft.Extensions.Logging;

public partial class KeeperDataImportCompleteHandler(
    ILogger<KeeperDataImportCompleteHandler> logger)
    : IMessageHandler<KeeperDataImportComplete>
{
    public Task<MessageProcessStatus> HandleAsync(
        MessageEnvelope<KeeperDataImportComplete> messageEnvelope,
        CancellationToken token = default)
    {
        LogProcessingKeeperDataImportCompleteMessage();

        // TODO: Implement actual logic to handle the message.
        return Task.FromResult(MessageProcessStatus.Success());
    }
}

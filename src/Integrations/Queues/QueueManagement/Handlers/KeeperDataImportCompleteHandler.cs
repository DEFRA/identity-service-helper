// <copyright file="KeeperDataImportCompleteHandler.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.QueueManagement.Handlers;

using AWS.Messaging;
using Defra.Identity.Ingest;
using Defra.Identity.QueueManagement.Messages;
using Microsoft.Extensions.Logging;

public class KeeperDataImportCompleteHandler(ILogger<KeeperDataImportCompleteHandler> logger, IIngestDataService service) : IMessageHandler<KeeperDataImportComplete>
{
    public async Task<MessageProcessStatus> HandleAsync(MessageEnvelope<KeeperDataImportComplete> messageEnvelope, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Processing KeeperDataImportComplete message.");

        var success = await service.Execute();

        if (success)
        {
            return MessageProcessStatus.Success();
        }

        return MessageProcessStatus.Failed();
    }
}

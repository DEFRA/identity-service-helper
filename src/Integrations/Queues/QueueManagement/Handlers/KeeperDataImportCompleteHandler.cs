// <copyright file="KeeperDataImportCompleteHandler.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.QueueManagement.Handlers;

using AWS.Messaging;
using Defra.Identity.Ingest;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.QueueManagement.Messages;
using Microsoft.Extensions.Logging;

public class KeeperDataImportCompleteHandler(ILogger<KeeperDataImportCompleteHandler> logger, IIngestDataService<CountyParishHoldings> cphService, IIngestDataService<Roles> rolesService) : IMessageHandler<KeeperDataImportComplete>
{
    public async Task<MessageProcessStatus> HandleAsync(MessageEnvelope<KeeperDataImportComplete> messageEnvelope, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Processing KeeperDataImportComplete message.");

        var cphSuccess = await cphService.Execute();
        var rolesSuccess = await rolesService.Execute();

        if (cphSuccess && rolesSuccess)
        {
            return MessageProcessStatus.Success();
        }

        return MessageProcessStatus.Failed();
    }
}

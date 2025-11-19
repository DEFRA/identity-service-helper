// <copyright file="KeeperDataImportedHandler.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Livestock.Auth.Services.Messaging.Handlers;

using Amazon.SQS;
using Amazon.SQS.Model;
using Livestock.Auth.Services.Messaging.Messages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Utils;

public class KeeperDataImportedHandler(
    ILogger<KeeperDataImportedHandler> logger)
{
    public async Task<bool> HandleAsync(
        Message message,
        IAmazonSQS sqs,
        CancellationToken token)
    {
        using var stopwatch = new StopwatchLogger(logger);
        KeeperDataImportedMessage? tmp = JsonConvert.DeserializeObject<KeeperDataImportedMessage>(message.Body);
        if (tmp is null)
        {
            throw new InvalidOperationException("Unable to deserialize message");
        }

        logger.LogInformation("Received {id} message {Message} from queue {QueueUrl}", tmp.MessageId, tmp.Message, message.ReceiptHandle);

        return true;
    }
}

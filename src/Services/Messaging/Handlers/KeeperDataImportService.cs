// <copyright file="KeeperDataImportService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Livestock.Auth.Services.Messaging.Handlers;

using Amazon.SQS;
using Amazon.SQS.Model;
using Livestock.Auth.Services.Messaging.Config;
using Livestock.Auth.Services.Messaging.Messages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

public class KeeperDataImportService : BackgroundService
{
    private readonly string queueUrl;
    private readonly IAmazonSQS sqs;
    private readonly ILogger<KeeperDataImportService> logger;
    private readonly KeeperDataImportedHandler handler;
    private readonly List<string> supportedMessageTypes;

    public KeeperDataImportService(
        IAmazonSQS sqs,
        KeeperDataImportedHandler handler,
        IOptions<IntakeQueueOptions> queueOptions,
        ILogger<KeeperDataImportService> logger)
    {
        this.sqs = sqs;
        this.queueUrl = queueOptions.Value.Url;
        this.supportedMessageTypes = queueOptions.Value.SupportedMessageTypes;
        this.logger = logger;
        this.handler = handler;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var resp = await sqs.ReceiveMessageAsync(
                new ReceiveMessageRequest
                {
                    QueueUrl = queueUrl,
                    WaitTimeSeconds = 20,
                    MaxNumberOfMessages = 1,
                },
                cancellationToken);

            if (resp?.Messages is null || resp.Messages.Count == 0)
            {
                logger.LogDebug("No messages received from queue {QueueUrl}", queueUrl);
                continue;
            }

            try
            {
                logger.LogInformation("Received {MessageCount} message(s) from queue {QueueUrl}", resp.Messages.Count, queueUrl);
                foreach (var message in resp.Messages)
                {
                    var envelope = JsonConvert.DeserializeObject<SnsEnvelope>(message.Body);
                    if (envelope is not null && supportedMessageTypes.Contains(envelope.TopicArn.Split(':').Last()))
                    {
                        await sqs.DeleteMessageAsync(queueUrl, message.ReceiptHandle, cancellationToken);
                        if (!await handler.HandleAsync(message, sqs, cancellationToken).ConfigureAwait(false))
                        {
                            logger.LogError("Failed to process message");
                        }
                    }
                    else
                    {
                        logger.LogWarning("Unsupported message type {MessageType} received.", envelope?.TopicArn);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing message");
            }
        }
    }
}

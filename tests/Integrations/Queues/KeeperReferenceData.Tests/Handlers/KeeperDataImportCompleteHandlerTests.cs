// <copyright file="KeeperDataImportCompleteHandlerTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Tests.Handlers;

using AWS.Messaging;
using Defra.Identity.Ingest;
using Defra.Identity.QueueManagement.Handlers;
using Defra.Identity.QueueManagement.Messages;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

public class KeeperDataImportCompleteHandlerTests
{
    private readonly ILogger<KeeperDataImportCompleteHandler> logger;
    private readonly KeeperDataImportCompleteHandler handler;
    private readonly IIngestDataService ingestDataService;

    public KeeperDataImportCompleteHandlerTests()
    {
        logger = Substitute.For<ILogger<KeeperDataImportCompleteHandler>>();
        ingestDataService = Substitute.For<IIngestDataService>();
        handler = new KeeperDataImportCompleteHandler(logger, ingestDataService);
    }

    [Fact]
    public async Task HandleAsync_ReturnsSuccess_WhenServiceSucceeds()
    {
        // Arrange
        var message = new KeeperDataImportComplete();
        var messageEnvelope = new MessageEnvelope<KeeperDataImportComplete>
        {
            Message = message,
        };

        ingestDataService.Execute().Returns(true);

        // Act
        var result = await handler.HandleAsync(messageEnvelope, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(MessageProcessStatus.Success().ToString(), result.ToString());
        logger.VerifyLogContainsOne(LogLevel.Information, "Processing KeeperDataImportComplete message.");
    }

    [Fact]
    public async Task HandleAsync_ReturnsFailed_WhenServiceFails()
    {
        // Arrange
        var message = new KeeperDataImportComplete();
        var messageEnvelope = new MessageEnvelope<KeeperDataImportComplete>
        {
            Message = message,
        };

        ingestDataService.Execute().Returns(false);

        // Act
        var result = await handler.HandleAsync(messageEnvelope, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(MessageProcessStatus.Failed().ToString(), result.ToString());
        logger.VerifyLogContainsOne(LogLevel.Information, "Processing KeeperDataImportComplete message.");
    }
}

// <copyright file="KeeperDataImportCompleteHandlerTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Tests.Handlers;

using AWS.Messaging;
using Defra.Identity.KeeperReferenceData.Handlers;
using Defra.Identity.KeeperReferenceData.Messages;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

public class KeeperDataImportCompleteHandlerTests
{
    private readonly ILogger<KeeperDataImportCompleteHandler> logger;
    private readonly KeeperDataImportCompleteHandler handler;

    public KeeperDataImportCompleteHandlerTests()
    {
        logger = Substitute.For<ILogger<KeeperDataImportCompleteHandler>>();
        handler = new KeeperDataImportCompleteHandler(logger);
    }

    [Fact]
    public async Task HandleAsync_LogsInformationAndReturnsSuccess()
    {
        // Arrange
        var message = new KeeperDataImportComplete();
        var messageEnvelope = new MessageEnvelope<KeeperDataImportComplete>
        {
            Message = message,
        };

        // Act
        var result = await handler.HandleAsync(messageEnvelope, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(MessageProcessStatus.Success().ToString(), result.ToString());
        logger.VerifyLogContainsOne(LogLevel.Information, "Processing KeeperDataImportComplete message.");
    }
}

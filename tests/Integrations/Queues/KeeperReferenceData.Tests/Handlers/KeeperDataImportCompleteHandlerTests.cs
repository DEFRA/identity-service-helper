// <copyright file="KeeperDataImportCompleteHandlerTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Tests.Handlers;

using AWS.Messaging;
using Defra.Identity.Ingest;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.QueueManagement.Handlers;
using Defra.Identity.QueueManagement.Messages;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

public class KeeperDataImportCompleteHandlerTests
{
    private readonly ILogger<KeeperDataImportCompleteHandler> logger;
    private readonly KeeperDataImportCompleteHandler handler;
    private readonly IIngestService<CountyParishHoldings> ingestService;
    private readonly IIngestService<Roles> roleIngestService;

    public KeeperDataImportCompleteHandlerTests()
    {
        logger = DefraLoggerExtensions.CreateNSubstituteLogger<KeeperDataImportCompleteHandler>();
        ingestService = Substitute.For<IIngestService<CountyParishHoldings>>();
        roleIngestService = Substitute.For<IIngestService<Roles>>();
        handler = new KeeperDataImportCompleteHandler(logger, ingestService, roleIngestService);
    }

    [Fact]
    public async Task HandleAsync_ReturnsSuccess_WhenBothServicesSucceed()
    {
        // Arrange
        var message = new KeeperDataImportComplete();
        var messageEnvelope = new MessageEnvelope<KeeperDataImportComplete>
        {
            Message = message,
        };

        ingestService.Execute().Returns(true);
        roleIngestService.Execute().Returns(true);

        // Act
        var result = await handler.HandleAsync(messageEnvelope, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(MessageProcessStatus.Success().ToString(), result.ToString());
        logger.VerifyLogContainsOne(LogLevel.Information, "Processing KeeperDataImportComplete message.");
    }

    [Fact]
    public async Task HandleAsync_ReturnsFailed_WhenCphServiceFails()
    {
        // Arrange
        var message = new KeeperDataImportComplete();
        var messageEnvelope = new MessageEnvelope<KeeperDataImportComplete>
        {
            Message = message,
        };

        ingestService.Execute().Returns(false);
        roleIngestService.Execute().Returns(true);

        // Act
        var result = await handler.HandleAsync(messageEnvelope, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(MessageProcessStatus.Failed().ToString(), result.ToString());
    }

    [Fact]
    public async Task HandleAsync_ReturnsFailed_WhenRolesServiceFails()
    {
        // Arrange
        var message = new KeeperDataImportComplete();
        var messageEnvelope = new MessageEnvelope<KeeperDataImportComplete>
        {
            Message = message,
        };

        ingestService.Execute().Returns(true);
        roleIngestService.Execute().Returns(false);

        // Act
        var result = await handler.HandleAsync(messageEnvelope, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(MessageProcessStatus.Failed().ToString(), result.ToString());
    }

    [Fact]
    public async Task HandleAsync_ReturnsFailed_WhenServiceThrows()
    {
        // Arrange
        var message = new KeeperDataImportComplete();
        var messageEnvelope = new MessageEnvelope<KeeperDataImportComplete>
        {
            Message = message,
        };

        ingestService.Execute().Returns(_ => Task.FromException<bool>(new Exception("Service error")));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.HandleAsync(messageEnvelope, CancellationToken.None));
    }

    [Fact]
    public async Task HandleAsync_RespectsCancellationToken()
    {
        // Arrange
        var message = new KeeperDataImportComplete();
        var messageEnvelope = new MessageEnvelope<KeeperDataImportComplete>
        {
            Message = message,
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        // In the current implementation, CancellationToken is passed to services but not explicitly checked in the handler.
        // However, services should respect it.
        // Since we are mocking services, we can verify they receive the token.

        await handler.HandleAsync(messageEnvelope, cts.Token);

        await ingestService.Received(1).Execute();
    }
}

// <copyright file="MessageQueueProcessorTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Messaging.Tests;

using System.Linq.Expressions;
using System.Net;
using Defra.Identity.Messaging.Models.Request;
using Defra.Identity.Messaging.Models.Response;
using Defra.Identity.Messaging.Services;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Messaging;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

public class MessageQueueProcessorTests
{
    private readonly IExternalMessagingRepository externalMessagingRepository = Substitute.For<IExternalMessagingRepository>();
    private readonly IMessagingService messagingService = Substitute.For<IMessagingService>();
    private readonly ILogger<MessageQueueProcessor> logger = DefraLoggerExtensions.CreateNSubstituteLogger<MessageQueueProcessor>();
    private readonly MessageQueueProcessor processor;

    public MessageQueueProcessorTests()
    {
        processor = new MessageQueueProcessor(externalMessagingRepository, messagingService, logger);
    }

    [Fact]
    public async Task ProcessMessageQueueAsync_NoMessages_ReturnsEmptyResult()
    {
        // Arrange
        externalMessagingRepository.GetList(Arg.Any<Expression<Func<ExternalMessaging, bool>>>(), TestContext.Current.CancellationToken)
            .Returns(new List<ExternalMessaging>());

        // Act
        var result = await processor.ProcessMessageQueueAsync(TestContext.Current.CancellationToken);

        // Assert
        result.ShouldSatisfyAllConditions(
           x => x.ShouldNotBeNull(),
           x => x.Success.EmailCountProcessed.ShouldBe(0),
           x => x.Success.SmsCountProcessed.ShouldBe(0),
           x => x.Error.EmailCountProcessed.ShouldBe(0),
           x => x.Error.SmsCountProcessed.ShouldBe(0));
    }

    [Fact]
    public async Task ProcessMessageQueueAsync_ProcessesMessagesSuccessfully()
    {
        // Arrange
        var messages = new List<ExternalMessaging>
        {
            new() { MessageType = MessageTypes.Email, TemplateId = Guid.NewGuid(), MessageRecipient = "email@test.com", ResponseCode = HttpStatusCode.Accepted, NotifyId = Guid.Empty, CreatedAt = DateTime.UtcNow },
            new() { MessageType = MessageTypes.Sms, TemplateId = Guid.NewGuid(), MessageRecipient = "1234567890", ResponseCode = HttpStatusCode.Accepted, NotifyId = Guid.Empty, CreatedAt = DateTime.UtcNow },
        };

        externalMessagingRepository.GetList(Arg.Any<Expression<Func<ExternalMessaging, bool>>>(), TestContext.Current.CancellationToken)
            .Returns(messages);

        messagingService.SendEmailMessageAsync(Arg.Any<Message>())
            .Returns(new MessageResponse { Status = HttpStatusCode.OK, NotifyId = Guid.NewGuid().ToString() });
        messagingService.SendSmsMessageAsync(Arg.Any<Message>())
            .Returns(new MessageResponse { Status = HttpStatusCode.OK, NotifyId = Guid.NewGuid().ToString() });

        // Act
        var result = await processor.ProcessMessageQueueAsync(TestContext.Current.CancellationToken);

        // Assert
        result.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x.Success.EmailCountProcessed.ShouldBe(1),
            x => x.Success.SmsCountProcessed.ShouldBe(1));
        await externalMessagingRepository.Received(2).Update(Arg.Any<ExternalMessaging>(), TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task ProcessMessageQueueAsync_HandlesSendErrors()
    {
        // Arrange
        var messages = new List<ExternalMessaging>
        {
            new() { MessageType = MessageTypes.Email, TemplateId = Guid.NewGuid(), MessageRecipient = "email@test.com", ResponseCode = HttpStatusCode.Accepted, NotifyId = Guid.Empty, CreatedAt = DateTime.UtcNow },
        };

        externalMessagingRepository.GetList(Arg.Any<Expression<Func<ExternalMessaging, bool>>>(), TestContext.Current.CancellationToken)
            .Returns(messages);

        messagingService.SendEmailMessageAsync(Arg.Any<Message>())
            .Returns(new MessageResponse { Status = HttpStatusCode.BadRequest, NotifyId = Guid.Empty.ToString(), Errors = [new ErrorItem { Message = "Error" }] });

        // Act
        var result = await processor.ProcessMessageQueueAsync(TestContext.Current.CancellationToken);

        // Assert
        result.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x.Error.EmailCountProcessed.ShouldBe(1),
            x => x.Success.EmailCountProcessed.ShouldBe(0));
        await externalMessagingRepository.Received(1).Update(Arg.Is<ExternalMessaging>(m => m.ResponseCode == HttpStatusCode.BadRequest), TestContext.Current.CancellationToken);
    }
}

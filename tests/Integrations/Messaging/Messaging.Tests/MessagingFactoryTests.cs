// <copyright file="MessagingFactoryTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Messaging.Tests;

using Defra.Identity.Messaging.Models.Request;
using Defra.Identity.Messaging.Models.Response;
using Defra.Identity.Messaging.Services;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Messaging;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

public class MessagingFactoryTests
{
    private readonly ILogger<MessagingFactory> logger = DefraLoggerExtensions.CreateNSubstituteLogger<MessagingFactory>();
    private readonly IMessagingService messagingService = Substitute.For<IMessagingService>();
    private readonly MessagingFactory service;
    private readonly IExternalMessagingRepository msgRepository
        = Substitute.For<IExternalMessagingRepository>();

    private readonly ICountyParishHoldingDelegationsNotificationsRepository notificationRepository
        = Substitute.For<ICountyParishHoldingDelegationsNotificationsRepository>();

    public MessagingFactoryTests()
    {
        msgRepository
            .Create(Arg.Any<ExternalMessaging>(), Arg.Any<CancellationToken>())
            .Returns(x => x.Arg<ExternalMessaging>());

        messagingService
            .SendEmailMessageAsync(Arg.Any<Message>())
            .Returns(new MessageResponse
            {
                NotifyId = Guid.NewGuid().ToString(), Status = System.Net.HttpStatusCode.OK,
            });

        notificationRepository
            .Create(Arg.Any<CountyParishHoldingDelegationsNotifications>(), Arg.Any<CancellationToken>())
            .Returns(x => x.Arg<CountyParishHoldingDelegationsNotifications>());

        service = new MessagingFactory(
            logger,
            messagingService,
            msgRepository,
            notificationRepository);
    }

    [Fact]
    public async Task SendEmailMessageAsync_ReturnsCorrectStatus()
    {
        // Arrange
        var request = new DelegationEmailMessage(
            Guid.Parse("dd000004-0000-4000-8000-000000000004"),
            MessageTemplateTypes.Delegation.DelegationInviteeConfirmation)
        {
            Recipient = "test1@test.com",
            Payload =
            {
                { "name", "Test User 100" },
            },
        };

        // Act
        await service.SendDelegationEmailAsync(request, TestContext.Current.CancellationToken);

        // Assert
        await msgRepository.Received(1).Create(Arg.Any<ExternalMessaging>(), TestContext.Current.CancellationToken);
        await msgRepository.Received(1).Update(Arg.Any<ExternalMessaging>(), TestContext.Current.CancellationToken);
        await notificationRepository.Received(1).Create(
            Arg.Any<CountyParishHoldingDelegationsNotifications>(),
            TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task QueueDelegationEmailAsync_ReturnsCorrectStatus()
    {
        // Arrange
        var request = new DelegationEmailMessage(
            Guid.Parse("dd000004-0000-4000-8000-000000000004"),
            MessageTemplateTypes.Delegation.DelegationInviteeConfirmation)
        {
            Recipient = "test1@test.com",
            Payload =
            {
                { "name", "Test User 100" },
            },
        };

        // Act
        await service.QueueDelegationEmailAsync(request, TestContext.Current.CancellationToken);

        // Assert
        await msgRepository.Received(1).Create(Arg.Any<ExternalMessaging>(), TestContext.Current.CancellationToken);
        await notificationRepository.Received(1).Create(
            Arg.Any<CountyParishHoldingDelegationsNotifications>(),
            TestContext.Current.CancellationToken);
        await messagingService.DidNotReceive().SendEmailMessageAsync(Arg.Any<Message>());
    }

    [Fact]
    public async Task SendDelegationEmailAsync_WhenServiceFails_UpdatesWithErrorCode()
    {
        // Arrange
        var request = new DelegationEmailMessage(
            Guid.Parse("dd000004-0000-4000-8000-000000000004"),
            MessageTemplateTypes.Delegation.DelegationInviteeConfirmation)
        {
            Recipient = "test1@test.com",
        };

        messagingService.SendEmailMessageAsync(Arg.Any<Message>())
            .Returns(new MessageResponse
            {
                Status = System.Net.HttpStatusCode.BadRequest,
                NotifyId = Guid.Empty.ToString(),
                Errors = [new ErrorItem { Message = "Error message" }],
            });

        // Act
        var result = await service.SendDelegationEmailAsync(request, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        await msgRepository.Received(1).Update(
            Arg.Is<ExternalMessaging>(m => m.ResponseCode == System.Net.HttpStatusCode.BadRequest),
            TestContext.Current.CancellationToken);
    }
}

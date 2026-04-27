// <copyright file="MessagingFactoryTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Messaging;

using Defra.Identity.Models;
using Defra.Identity.Models.Requests.Delegations;
using Defra.Identity.Models.Requests.Messaging;
using Defra.Identity.Models.Responses.Messaging;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Messaging;
using Defra.Identity.Services.Messaging;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class MessagingFactoryTests
{
    private readonly ILogger<MessagingFactory> logger = Substitute.For<ILogger<MessagingFactory>>();
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
            .SendEmailMessage(Arg.Any<Message>())
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
}

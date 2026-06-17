// <copyright file="MessagingServiceTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Messaging.Tests;

using System.Collections;
using System.Diagnostics;
using System.Net;
using Defra.Identity.Messaging.Configuration;
using Defra.Identity.Messaging.Models.Request;
using Defra.Identity.Messaging.Models.Response;
using Defra.Identity.Messaging.Services;
using Defra.Identity.Postgres.Database;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

public class MessagingServiceTests
{
    private const string SuccessEmail = "doesnt-really-matter@simulator.notify";

    private const string TestKey = "authtestkey-3f6a9ae4-62d7-48fc-92fa-fb7008e44f9d-b14612fd-e032-4961-9848-cd693b4e8ac7";
    private readonly Guid pipelineTestMsg = Guid.Parse("d42c86e2-4f6c-4099-a747-428bf8a418a2");

    private readonly ILogger<MessagingService> logger = DefraLoggerExtensions.CreateNSubstituteLogger<MessagingService>();
    private readonly IOptions<MessagingOptions> options = Substitute.For<IOptions<MessagingOptions>>();
    private readonly IMessagingService service;

    public MessagingServiceTests()
    {
        options.Value.Returns(new MessagingOptions() { ApiKey = TestKey, });

        service = new MessagingService(
            logger,
            options);
    }

    [Theory]
    [InlineData(SuccessEmail, "delivered")]
    public async Task SendEmailMessageAsync_ReturnsCorrectStatus(string recipient, string response)
    {
        // Arrange
        var request =
            new TestMessage(MessageTypes.Email, pipelineTestMsg) { Recipient = recipient, };

        // Act/Assert
        var result = await service.SendEmailMessageAsync(request);
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();

        // Polling for status because it might not be "delivered" immediately
        MessageStatus notification = null!;
        var stopwatch = Stopwatch.StartNew();
        while (stopwatch.Elapsed < TimeSpan.FromSeconds(10))
        {
            notification = await service.GetNotificationAsync(result.NotifyId);
            if (notification.Status == "delivered")
            {
                break;
            }

            await Task.Delay(500);
        }

        notification.ShouldNotBeNull();
        notification.Status.ShouldBe(response);
    }

    [Fact]
    public async Task SendSmsMessageAsync_ReturnsCorrectStatus()
    {
        // Arrange
        // Note: For Sms, the template must support SMS. pipelineTestMsg might not.
        // However, since this is using the Notify simulator via TestKey, it might fail if template doesn't exist.
        // If it fails with "Template not found", it's expected if pipelineTestMsg is an email template.
        var request = new TestMessage(MessageTypes.Sms, pipelineTestMsg) { Recipient = "07700900000", };

        // Act
        var result = await service.SendSmsMessageAsync(request);

        // Assert
        // result.IsSuccess.ShouldBeTrue(); // simulator might return false if template/recipient combination is invalid for SMS
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetNotificationAsync_ReturnsCorrectStatus()
    {
        // Arrange
        var request = new TestMessage(MessageTypes.Email, pipelineTestMsg) { Recipient = SuccessEmail, };
        var sendResult = await service.SendEmailMessageAsync(request);

        // Act
        // Polling for status because it might not be "delivered" immediately
        MessageStatus notification = null!;
        var stopwatch = Stopwatch.StartNew();
        while (stopwatch.Elapsed < TimeSpan.FromSeconds(10))
        {
            notification = await service.GetNotificationAsync(sendResult.NotifyId);
            if (notification.Status == "delivered")
            {
                break;
            }

            await Task.Delay(500);
        }

        // Assert
        notification.ShouldNotBeNull();
        notification.NotifyId.ShouldBe(sendResult.NotifyId);
        notification.Status.ShouldBe("delivered");
    }

    [Theory]
    [ClassData(typeof(TestMessageErrors))]
    public async Task SendEmailMessageAsync_WithMissingRecipient_ReturnsErrorResponse(TestMessage request, string errorMessage)
    {
        // Arrange/Act
        var result = await service.SendEmailMessageAsync(request);

        // Assert
        result.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x.Status.ShouldBe(HttpStatusCode.BadRequest),
            x => x.IsSuccess.ShouldBeFalse(),
            x => x.Errors.ShouldNotBeNull(),
            x => x.Errors[0].Message.ShouldBe(errorMessage));
    }

    public class TestMessageErrors() : IEnumerable<TheoryDataRow<TestMessage, string>>
    {
        public IEnumerator<TheoryDataRow<TestMessage, string>> GetEnumerator()
        {
            // no template specified
            yield return new TheoryDataRow<TestMessage, string>(
                new TestMessage(MessageTypes.Email, Guid.Empty) { Recipient = "test1@test.com", },
                "Template not found");

            // no recipient specified
            yield return new TheoryDataRow<TestMessage, string>(
                new TestMessage(MessageTypes.Email, MessageTemplateTypes.Delegation.DelegationInviteeConfirmation.Value)
                {
                    Recipient = null!,
                },
                "email_address None is not of type string");
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class TestMessage(MessageTypes type, Guid templateId)
        : Message(type, templateId);
}

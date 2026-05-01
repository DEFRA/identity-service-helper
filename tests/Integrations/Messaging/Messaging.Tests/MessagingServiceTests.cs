// <copyright file="MessagingServiceTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Messaging.Tests;

using System.Collections;
using System.Net;
using Defra.Identity.Messaging.Configuration;
using Defra.Identity.Messaging.Models;
using Defra.Identity.Messaging.Models.Request;
using Defra.Identity.Messaging.Services;
using Defra.Identity.Models;
using Defra.Identity.Postgres.Database;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

public class MessagingServiceTests
{
    private const string TemporaryFailureEmail = "temp-fail@simulator.notify";
    private const string PermanentFailureEmail = "perm-fail@simulator.notify";
    private const string SuccessEmail = "doesnt-really-matter@simulator.notify";

    private const string TestKey = "authtestkey-3f6a9ae4-62d7-48fc-92fa-fb7008e44f9d-b14612fd-e032-4961-9848-cd693b4e8ac7";

    private readonly ILogger<MessagingService> logger = Substitute.For<ILogger<MessagingService>>();
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
    [InlineData(TemporaryFailureEmail, "temporary-failure")]
    [InlineData(PermanentFailureEmail, "permanent-failure")]
    [InlineData(SuccessEmail, "delivered")]
    public async Task SendEmailMessageAsync_ReturnsCorrectStatus(string recipient, string response)
    {
        // Arrange
        var request = new TestMessage(MessageTypes.Email, MessageTemplateTypes.Delegation.DelegationInviteeConfirmation.Value) { Recipient = recipient, };

        // Act
        var result = await service.SendEmailMessageAsync(request);
        await Task.Delay(1000, TestContext.Current.CancellationToken);
        var notification = await service.GetNotificationAsync(result.NotifyId);

        // Assert
        result.ShouldNotBeNull();
        notification.ShouldNotBeNull();
        notification.Status.ShouldBe(response);
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

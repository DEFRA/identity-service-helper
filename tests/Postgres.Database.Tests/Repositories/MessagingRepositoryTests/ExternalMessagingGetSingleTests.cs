// <copyright file="ExternalMessagingGetSingleTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.MessagingRepositoryTests;

using System.ComponentModel;
using System.Linq.Expressions;
using System.Net;
using Defra.Identity.Models;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Messaging;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class ExternalMessagingGetSingleTests(PostgreContainerFixture fixture)
    : BaseTests(fixture)
{
    [Fact]
    [Description("Should get a single entity")]
    public async Task ShouldGetSingleEntity()
    {
        // Arrange
        var logger = Substitute.For<ILogger<ExternalMessagingRepository>>();
        var repository = new ExternalMessagingRepository(Context, ReadOnlyContext, logger);

        Expression<Func<ExternalMessaging, bool>> filter = msg => msg.Id == 1;

        // Act
        var entity = await repository.GetSingle(filter, TestContext.Current.CancellationToken);

        // Assert
        logger.Received(1).Log(LogLevel.Information, "Getting single external messaging record");

        entity.ShouldNotBeNull();

        entity.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(1),
            (x) => x.MessageType.ShouldBe(MessageTypes.Email),
            (x) => x.MessageRecipient.ShouldBe("test1@test.com"),
            (x) => x.TemplateId.ShouldBe(Guid.Parse("9edac4c9-9b29-4ea6-9e37-6ed32776a943")),
            (x) => x.NotifyId.ShouldBe(Guid.Parse("550e8400-e29b-41d4-a716-446655440001")),
            (x) => x.RequestPayload.ShouldBe("""{ "message": "test message" }"""),
            (x) => x.SentAt.ShouldBe(DateTime.Parse("2020-01-01").ToUniversalTime()),
            (x) => x.ResponseCode.ShouldBe(HttpStatusCode.OK),
            (x) => x.ResponseMessage.ShouldBe("OK"),
            (x) => x.ExceptionMessage.ShouldBeEmpty(),
            (x) => x.CreatedAt.ShouldBe(DateTime.Parse("2020-01-01").ToUniversalTime()),
            (x) => x.CreatedById.ShouldBe(AdminUserId));
    }

    [Fact]
    [Description("Should return null when entity does not exist")]
    public async Task ShouldReturnNullWhenEntityDoesNotExist()
    {
        // Arrange
        var logger = Substitute.For<ILogger<ExternalMessagingRepository>>();
        var repository = new ExternalMessagingRepository(Context, ReadOnlyContext, logger);

        Expression<Func<ExternalMessaging, bool>> filter = msg => msg.Id == 9999;

        // Act
        var entity = await repository.GetSingle(filter, TestContext.Current.CancellationToken);

        // Assert
        logger.Received(1).Log(LogLevel.Information, "Getting single external messaging record");

        entity.ShouldBeNull();
    }
}

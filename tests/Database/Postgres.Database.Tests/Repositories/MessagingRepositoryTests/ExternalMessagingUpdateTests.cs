// <copyright file="ExternalMessagingUpdateTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.MessagingRepositoryTests;

using System.ComponentModel;
using System.Net;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class ExternalMessagingUpdateTests(PostgreContainerFixture fixture)
    : BaseTests(fixture)
{
    [Fact]
    [Description("Should update a detached message when its created-by user is already tracked")]
    public async Task ShouldUpdateDetachedMessageWhenCreatedByUserAlreadyTracked()
    {
        // Arrange
        var logger = DefraLoggerExtensions.CreateNSubstituteLogger<ExternalMessagingRepository>();
        var repository = new ExternalMessagingRepository(Context, ReadOnlyContext, logger);

        var trackedUser = await Context.UserAccounts
            .SingleAsync(x => x.Id == AdminUserId, TestContext.Current.CancellationToken);

        var message = await repository.GetSingle(x => x.Id == 1, TestContext.Current.CancellationToken);

        message.ShouldNotBeNull();
        message.CreatedByUser.ShouldNotBeNull();
        message.CreatedByUser.ShouldNotBeSameAs(trackedUser);

        message.ResponseCode = HttpStatusCode.Accepted;
        message.ResponseMessage = "Accepted";

        // Act
        var result = await repository.Update(message, TestContext.Current.CancellationToken);

        // Assert
        result.ResponseCode.ShouldBe(HttpStatusCode.Accepted);
        result.ResponseMessage.ShouldBe("Accepted");

        logger.VerifyLogContainsOne(LogLevel.Information, $"Updating external messaging record with id {message.Id}");
    }
}

// <copyright file="AzureB2CSyncServiceTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Messaging.Tests.PollingProcessor;

using Defra.Identity.Messaging.PollingProcessor.Config;
using Defra.Identity.Messaging.PollingProcessor.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Quartz;

public class AzureB2CSyncServiceTests
{
    [Fact]
    public void ServiceCreates_AsExpected()
    {
        // Arrange
        var logger = Substitute.For<ILogger<AzureB2CSyncService>>();
        var options = Substitute.For<IOptions<AzureB2CSyncServiceConfiguration>>();

        // Act
        var sut = new AzureB2CSyncService(
            logger,
            options);

        // Assert
        sut.ShouldNotBeNull();
    }

    [Fact]
    public async Task EnsureExecute_WorksAsExpected()
    {
        // Arrange
        var logger = Substitute.For<ILogger<AzureB2CSyncService>>();
        var options = Substitute.For<IOptions<AzureB2CSyncServiceConfiguration>>();
        var context = Substitute.For<IJobExecutionContext>();
        var sut = new AzureB2CSyncService(logger, options);

        // Act
        await sut.Execute(context);

        // Assert
        sut.ShouldNotBeNull();
    }
}

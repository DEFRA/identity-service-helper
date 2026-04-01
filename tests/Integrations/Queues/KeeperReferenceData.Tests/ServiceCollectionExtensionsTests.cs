// <copyright file="ServiceCollectionExtensionsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Tests;

using Defra.Identity.Ingest;
using Defra.Identity.QueueManagement;
using Defra.Identity.QueueManagement.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddKeeperReferenceDataQueueIntegration_RegistersRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["QueueOptions:IntakeQueueOptions:Url"] = "https://sqs.us-east-1.amazonaws.com/123456789012/test-queue",
                ["QueueOptions:IntakeQueueOptions:WaitTimeSeconds"] = "20",
                ["QueueOptions:IntakeQueueOptions:MaxNumberOfMessages"] = "1",
                ["AWS:UseLocalStack"] = "false",
                ["AWS:Region"] = "eu-west-2",
                ["AWS:ServiceURL"] = "http://localhost:4566",
                ["AWS:AccessKey"] = "test",
                ["AWS:SecretKey"] = "test",
                ["KrdsApi:Url"] = "http://localhost:5062",
            })
            .Build();

        services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        services.AddDataIngestServices(configuration);
        services.AddKeeperRecordsDataIntegrationService(configuration);

        // Act
        services.AddKeeperReferenceDataQueueIntegration(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var messageHandler = serviceProvider.GetService<KeeperDataImportCompleteHandler>();
        Assert.NotNull(messageHandler);
    }

    [Fact]
    public void AddKeeperReferenceDataQueueIntegration_ThrowsException_WhenConfigurationSectionIsMissing()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => services.AddKeeperReferenceDataQueueIntegration(configuration));
    }
}

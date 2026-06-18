// <copyright file="ServiceCollectionExtensionsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Tests;

using Defra.Identity.Ingest;
using Defra.Identity.Postgres.Database;
using Defra.Identity.QueueManagement;
using Defra.Identity.QueueManagement.Handlers;
using Defra.Identity.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Shouldly;
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
                ["ConnectionStrings:PostgresConnection"] = "Host=localhost;Database=test;Username=test;Password=test",
                ["ConnectionStrings:ReadOnlyPostgresConnection"] = "Host=localhost;Database=test;Username=test;Password=test",
            })
            .Build();

        services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        var mockEnv = new Microsoft.Extensions.Hosting.Internal.HostingEnvironment
        {
            EnvironmentName = Environments.Development,
        };
        services.AddSingleton<IHostEnvironment>(mockEnv);
        services.AddPostgresDatabase(configuration);
        services.AddRepositories(configuration);
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

    [Fact]
    public void AddKeeperReferenceDataQueueIntegration_RegistersLocalStackSqs_WhenUseLocalStackIsTrue()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["QueueOptions:IntakeQueueOptions:Url"] = "http://localhost:4566/000000000000/test-queue",
                ["QueueOptions:IntakeQueueOptions:WaitTimeSeconds"] = "20",
                ["QueueOptions:IntakeQueueOptions:MaxNumberOfMessages"] = "1",
                ["AWS:UseLocalStack"] = "true",
                ["AWS:Region"] = "eu-west-2",
                ["AWS:ServiceURL"] = "http://localhost:4566",
                ["AWS:AccessKey"] = "test",
                ["AWS:SecretKey"] = "test",
            })
            .Build();

        services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        var mockEnv = new Microsoft.Extensions.Hosting.Internal.HostingEnvironment
        {
            EnvironmentName = Environments.Development,
        };
        services.AddSingleton<IHostEnvironment>(mockEnv);

        // Act
        services.AddKeeperReferenceDataQueueIntegration(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var sqsClient = serviceProvider.GetService<Amazon.SQS.IAmazonSQS>();
        sqsClient.ShouldNotBeNull();
    }

    [Fact]
    public void AddKeeperReferenceDataQueueIntegration_ThrowsArgumentNullException_WhenServicesIsNull()
    {
        // Arrange
        IServiceCollection services = null!;
        var configuration = Substitute.For<IConfiguration>();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => services.AddKeeperReferenceDataQueueIntegration(configuration));
    }

    [Fact]
    public void AddKeeperReferenceDataQueueIntegration_ThrowsArgumentNullException_WhenConfigurationIsNull()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration configuration = null!;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => services.AddKeeperReferenceDataQueueIntegration(configuration));
    }
}

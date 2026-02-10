// <copyright file="SqsServiceExtensionsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Messaging.Tests.MessageProcessor.Extensions;

using Amazon.SQS;
using Defra.Identity.Extensions;
using Defra.Identity.Messaging.MessageProcessor.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class SqsServiceExtensionsTests
{
    [Fact]
    public void Ensure_IfConfigPresent_ServiceRegistered()
    {
        // Arrange
        IServiceCollection tmpCollection = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AWS:UseLocalStack"] = "true",
                ["AWS:ServiceURL"] = "http://localhost:4566",
                ["AWS:Region"] = "eu-west-2",
                ["AWS:AccessKey"] = "test",
                ["AWS:SecretKey"] = "test",
                ["QueueOptions:IntakeQueueOptions:Url"] = "Set in cdp-app-config",
                ["QueueOptions:IntakeQueueOptions:WaitTimeSeconds"] = "20",
                ["QueueOptions:IntakeQueueOptions:MaxNumberOfMessages"] = "1",
                ["QueueOptions:IntakeQueueOptions:SupportedMessageTypes:0"] = "ls_keeper_data_import_complete",
            })
            .Build();

        // Act
        tmpCollection.AddMessageProcessorService(config);

        // Assert
        var list = tmpCollection.ToList();
        list.ShouldSatisfyAllConditions(
            l => l.ShouldContain(x =>
                x.ServiceType == typeof(IAmazonSQS)),
            l => l.ShouldContain(x =>
                x.ImplementationType == typeof(KeeperDataImportedHandler)));
    }

    [Fact]
    public void Ensure_IfConfigNotPresent_ThrowsException()
    {
        // Arrange
        IServiceCollection tmpCollection = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["NotAKeyWeNeed"] = "true",
            })
            .Build();

        // Act
        var e = Assert.Throws<InvalidOperationException>(() => tmpCollection.AddMessageProcessorService(config));

        // Assert
        e.Message.ShouldBe($"Section 'QueueOptions:IntakeQueueOptions' not found in configuration.");
    }
}

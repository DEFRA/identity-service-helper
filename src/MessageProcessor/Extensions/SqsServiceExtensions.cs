// <copyright file="SqsServiceExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

// ReSharper disable once CheckNamespace
namespace Defra.Identity.Extensions;

using Amazon.Runtime;
using Amazon.SQS;
using Defra.Identity.Config;
using Defra.Identity.MessageProcessor.Config;
using Defra.Identity.MessageProcessor.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class SqsServiceExtensions
{
    public static void AddMessageProcessorService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var intakeEventQueueConfig = configuration
            .GetRequiredSection($"{nameof(QueueOptions)}:{nameof(IntakeQueueOptions)}");

        var awsConfig = configuration.GetSection("AWS").Get<AwsConfiguration>();
        services
            .Configure<IntakeQueueOptions>(intakeEventQueueConfig)
            .AddTransient<KeeperDataImportedHandler>()
            .AddHostedService<KeeperDataImportService>();

        if (awsConfig is { UseLocalStack: true })
        {
            services.AddSingleton<IAmazonSQS>(_ => new AmazonSQSClient(
                new BasicAWSCredentials(
                    awsConfig.SecretKey,
                    awsConfig.AccessKey),
                new AmazonSQSConfig
                {
                    ServiceURL = awsConfig.ServiceUrl,
                    AuthenticationRegion = awsConfig.Region,
                }));
        }
        else
        {
            services.AddAWSService<IAmazonSQS>();
        }
    }
}

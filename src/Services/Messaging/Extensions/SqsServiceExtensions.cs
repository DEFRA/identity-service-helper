// <copyright file="SqsServiceExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Livestock.Auth.Services.Messaging.Extensions;

using Amazon.Runtime;
using Amazon.SQS;
using Livestock.Auth.Services.Config;
using Livestock.Auth.Services.Messaging.Config;
using Livestock.Auth.Services.Messaging.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class SqsServiceExtensions
{
    public static void AddMessagingDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var intakeEventQueueConfig = configuration
            .GetRequiredSection($"{nameof(QueueOptions)}:{nameof(IntakeQueueOptions)}");

        var awsConfig = configuration.GetSection("AWS").Get<AwsConfiguration>();
        services
            .Configure<IntakeQueueOptions>(intakeEventQueueConfig)
            .AddAWSService<IAmazonSQS>()
            .AddTransient<KeeperDataImportedHandler>()
            .AddHostedService<KeeperDataImportService>();

        if (awsConfig.UseLocalStack)
        {
            services.AddSingleton<IAmazonSQS>(_ => new AmazonSQSClient(
                new BasicAWSCredentials(
                    awsConfig.SecretKey,
                    awsConfig.AccessKey),
                new AmazonSQSConfig
                {
                    ServiceURL = awsConfig.ServiceURL,
                    AuthenticationRegion = awsConfig.Region,
                }));
        }
        else
        {
            services.AddAWSService<IAmazonSQS>();
        }
    }
}

// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.QueueManagement;

using Amazon.SQS;
using Defra.Identity.QueueManagement.Configuration;
using Defra.Identity.QueueManagement.Handlers;
using Defra.Identity.QueueManagement.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKeeperReferenceDataQueueIntegration(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var queueOptions = configuration.GetSection(nameof(QueueOptions)).Get<QueueOptions>();
        if (queueOptions == null)
        {
            throw new InvalidOperationException($"Configuration section '{nameof(QueueOptions)}' not found.");
        }

        var awsOptions = configuration.GetSection("AWS").Get<AwsOptions>() ?? new AwsOptions();

        services.Configure<QueueOptions>(configuration.GetSection(nameof(QueueOptions)));
        services.Configure<AwsOptions>(configuration.GetSection("AWS"));

        if (awsOptions.UseLocalStack)
        {
            services.AddSingleton<IAmazonSQS>(sp =>
            {
                return new AmazonSQSClient(new AmazonSQSConfig
                {
                    ServiceURL = awsOptions.ServiceUrl,
                    AuthenticationRegion = awsOptions.Region,
                });
            });
        }

        services.AddAWSMessageBus(builder =>
        {
            builder.AddSQSPoller(queueOptions.IntakeQueueOptions.Url, options =>
            {
                options.WaitTimeSeconds = queueOptions.IntakeQueueOptions.WaitTimeSeconds;
            });

            builder.AddMessageHandler<KeeperDataImportCompleteHandler, KeeperDataImportComplete>(QueueNames.KeeperDataImportComplete);
            builder.AddMessageSource(QueueNames.KeeperDataImportComplete);
        });

        return services;
    }
}

// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData;

using Defra.Identity.KeeperReferenceData.Configuration;
using Defra.Identity.KeeperReferenceData.Handlers;
using Defra.Identity.KeeperReferenceData.Messages;
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

        services.Configure<QueueOptions>(configuration.GetSection(nameof(QueueOptions)));

        services.AddAWSMessageBus(builder =>
        {
            builder.AddSQSPoller(queueOptions.IntakeQueueOptions.Url);

            builder.AddMessageHandler<KeeperDataImportCompleteHandler, KeeperDataImportComplete>("ls_keeper_data_import_complete");
        });

        return services;
    }
}

// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData;

using System.Net;
using System.Net.Http.Headers;
using Defra.Identity.KeeperReferenceData.Configuration;
using Defra.Identity.KeeperReferenceData.Exceptions;
using Defra.Identity.KeeperReferenceData.Handlers;
using Defra.Identity.KeeperReferenceData.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKeeperRecordsDataIntegrationService(this IServiceCollection services, IConfigurationRoot configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var krdsApiSection = configuration.GetSection(nameof(KrdsApi));
        if (!krdsApiSection.Exists())
        {
            throw new KeeperReferenceDataConfigurationException($"Configuration section '{nameof(KrdsApi)}' not found.");
        }

        var krdsApi = krdsApiSection.Get<KrdsApi>();
        if (krdsApi == null)
        {
            throw new KeeperReferenceDataConfigurationException($"Configuration section '{nameof(KrdsApi)}' is empty or invalid.");
        }

        services.Configure<KrdsApi>(krdsApiSection);

        services.AddTransient<KrdsAuthorizationHandler>();
        services.AddHttpClient<IKrdsTokenProvider, KrdsTokenProvider>()
            .AddPolicyHandler(GetRetryPolicy());

        services.AddHttpClient<IKrdsProvider, KrdsProvider>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(krdsApi.Url);
            })
            .AddHttpMessageHandler<KrdsAuthorizationHandler>()
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddPolicyHandler(GetRetryPolicy());

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
            .OrResult(msg => (int)msg.StatusCode == 429)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}

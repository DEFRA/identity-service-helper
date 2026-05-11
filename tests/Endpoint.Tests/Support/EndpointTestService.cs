// <copyright file="EndpointTestService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Endpoint.Tests.Support;

using System;
using Defra.Identity.Endpoint.Tests.Configuration;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// The test service for API.
/// </summary>
public static class EndpointTestService
{
    #pragma warning disable SA1306
    private static IServiceProvider? InternalServiceProvider;
    #pragma warning restore SA1306

    /// <summary>
    /// Gets the service provider for our Api Test Service.
    /// </summary>
    /// <value>The service Provider.</value>
    public static IServiceProvider ServiceProvider => InternalServiceProvider ??= BuildServiceProvider();

    private static IServiceProvider BuildServiceProvider()
    {
        IServiceCollection services = new ServiceCollection();

        IConfiguration testConfiguration = new ConfigurationBuilder()
            .AddJsonFile("testsettings.json", true)
            .AddJsonFile("testsettings.local.json", true)
            .AddEnvironmentVariables()
            .Build();

        services.AddSingleton(testConfiguration);
        services.AddOptions<ApiConfiguration>().BindConfiguration("ApiConfiguration");
        services.AddSingleton<PostgreContainerFixture>();
        return services.BuildServiceProvider();
    }
}

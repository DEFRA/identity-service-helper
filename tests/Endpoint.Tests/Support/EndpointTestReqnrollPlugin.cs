// <copyright file="EndpointTestReqnrollPlugin.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Endpoint.Tests.Support;

using Defra.Identity.Endpoint.Tests.Configuration;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Microsoft;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Reqnroll.BoDi;
using Reqnroll.Plugins;
using Reqnroll.UnitTestProvider;

/// <summary>
/// The Reqnroll plugin for this assembly. Allows us to connect up DI.
/// </summary>
public class EndpointTestReqnrollPlugin : IRuntimePlugin
{
    /// <inheritdoc />
    public void Initialize(
        RuntimePluginEvents runtimePluginEvents,
        RuntimePluginParameters runtimePluginParameters,
        UnitTestProviderConfiguration unitTestProviderConfiguration)
    {
        Requires.NotNull(runtimePluginEvents);

        runtimePluginEvents.RegisterGlobalDependencies += this.RegisterGlobalDependencies!;
    }

    private void RegisterGlobalDependencies(object sender, RegisterGlobalDependenciesEventArgs e)
    {
        var serviceProvider = EndpointTestService.ServiceProvider;
        serviceProvider.ShouldNotBeNull();

        var factory = new EndpointTestApplicationFactory();
        e.ObjectContainer.RegisterInstanceAs(factory);
        e.ObjectContainer.RegisterFactoryAs(oc => serviceProvider);
        e.ObjectContainer.RegisterFactoryAs(oc => serviceProvider.GetRequiredService<IConfiguration>());
        e.ObjectContainer.RegisterAllImplementations<ITestContext>();
        e.ObjectContainer.RegisterFactoryAs(oc => serviceProvider.GetRequiredService<IOptions<ApiConfiguration>>());
        e.ObjectContainer.RegisterFactoryAs(oc => serviceProvider.GetRequiredService<PostgreContainerFixture>());
    }
}

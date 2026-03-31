// <copyright file="EndpointTestReqnrollPlugin.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Endpoint.Tests.Support;

using System;
using System.Linq;
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
    private string[] testHostFileNames = { "TestHost.dll", "ReSharperTestRunner.dll" };

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
        // If the test host is not running, we don't need to register any dependencies.
        if (!Environment.GetCommandLineArgs().Any(arg =>
                this.testHostFileNames.Any(name => arg.Contains(name, StringComparison.OrdinalIgnoreCase))))
        {
            return;
        }

        var serviceProvider = EndpointTestService.ServiceProvider;

        var factory = new EndpointTestApplicationFactory();
        e.ObjectContainer.RegisterInstanceAs(factory);
        e.ObjectContainer.RegisterFactoryAs(oc => serviceProvider);
        e.ObjectContainer.RegisterFactoryAs(oc => serviceProvider.GetRequiredService<IConfiguration>());
        e.ObjectContainer.RegisterAllImplementations<ITestContext>();
        e.ObjectContainer.RegisterFactoryAs(oc => serviceProvider.GetRequiredService<IOptions<ApiConfiguration>>());
        e.ObjectContainer.RegisterFactoryAs(oc => serviceProvider.GetRequiredService<PostgreContainerFixture>());
    }
}

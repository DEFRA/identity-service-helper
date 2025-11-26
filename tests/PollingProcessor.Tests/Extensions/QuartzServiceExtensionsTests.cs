// <copyright file="QuartzServiceExtensionsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.PollingProcessor.Tests.Extensions;

using Defra.Identity.Extensions;
using Defra.Identity.PollingProcessor.Config;
using Defra.Identity.PollingProcessor.Tests.Fakes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Quartz;
using Quartz.Spi;

public class QuartzServiceExtensionsTests
{
    [Fact]
    public void Ensure_ServiceRegistered()
    {
        // Arrange
        IServiceCollection tmpCollection = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["PolledServices:AzureB2CSyncFake:ServiceType"] = "Defra.Identity.PollingProcessor.Tests.Fakes.FakePollingService",
                ["PolledServices:AzureB2CSyncFake:InterfaceType"] = "Defra.Identity.PollingProcessor.Tests.Fakes.IFakePollingService",
                ["PolledServices:AzureB2CSyncFake:ConfigurationType"] = "Defra.Identity.PollingProcessor.Tests.Fakes.FakePollingConfiguration",
                ["PolledServices:AzureB2CSyncFake:Description"] = "Fake Service",
                ["PolledServices:AzureB2CSyncFake:CronSchedule"] = "0 0/1 * * * ?",
            })
            .Build();

        // Act
        tmpCollection.AddPollingProcessorService(config);

        // Assert
        tmpCollection.Count.ShouldBeGreaterThan(20);
        var list = tmpCollection.ToList();
        list.ShouldContain(x => x.ServiceType == typeof(IFakePollingService));
    }
}

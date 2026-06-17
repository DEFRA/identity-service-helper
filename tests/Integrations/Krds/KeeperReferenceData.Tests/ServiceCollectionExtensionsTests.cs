// <copyright file="ServiceCollectionExtensionsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Tests;

using Defra.Identity.KeeperReferenceData.Configuration;
using Defra.Identity.KeeperReferenceData.Exceptions;
using Defra.Identity.KeeperReferenceData.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddKeeperRecordsDataIntegrationService_RegistersRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["KrdsApi:Url"] = "http://localhost:5062",
                ["KrdsApi:TokenUrl"] = "http://localhost:5062/token",
                ["KrdsApi:ClientId"] = "id",
                ["KrdsApi:ClientSecret"] = "secret",
            })
            .Build();

        // Act
        services.AddKeeperRecordsDataIntegrationService(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        serviceProvider.ShouldSatisfyAllConditions(
            x => x.GetService<IKrdsTokenProvider>().ShouldNotBeNull(),
            x => x.GetService<IKrdsProvider>().ShouldNotBeNull());

        var options = serviceProvider.GetRequiredService<IOptions<KrdsApi>>();

        options.ShouldSatisfyAllConditions(
            x => x.Value.Url.ShouldNotBeNull(),
            x => x.Value.TokenUrl.ShouldNotBeNull(),
            x => x.Value.ClientId.ShouldNotBeNull(),
            x => x.Value.ClientSecret.ShouldNotBeNull(),
            x => x.Value.Url.ShouldBe("http://localhost:5062"));
    }

    [Fact]
    public void AddKeeperRecordsDataIntegrationService_Throws_When_Configuration_Missing()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        // Act & Assert
        Assert.Throws<KeeperReferenceDataConfigurationException>(() => services.AddKeeperRecordsDataIntegrationService(configuration));
    }
}

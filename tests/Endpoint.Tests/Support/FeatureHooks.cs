// <copyright file="FeatureHooks.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Endpoint.Tests.Support;

using System.Diagnostics.CodeAnalysis;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Reqnroll;

[ExcludeFromCodeCoverage]
[Binding]
public class FeatureHooks
{
    protected FeatureHooks()
    {
    }

    [BeforeFeature]
    public static async Task RegisterFeatureDependencies(FeatureContext featureContext)
    {
        featureContext.FeatureContainer.RegisterFactoryAs<StringProcessor>(
            _ => new StringProcessor(featureContext));

        var pgFixture = featureContext.FeatureContainer.Resolve<PostgreContainerFixture>();
        await pgFixture.InitializeAsync();
    }
}

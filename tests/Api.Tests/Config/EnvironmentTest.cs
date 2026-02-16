// <copyright file="EnvironmentTest.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Tests.Config;

using Microsoft.AspNetCore.Builder;
using Environment = Defra.Identity.Api.Config.Environment;

public class EnvironmentTest
{
    [Fact]
    public void IsNotDevModeByDefault()
    {
        var builder = WebApplication.CreateEmptyBuilder(new WebApplicationOptions());
        var isDev = Environment.IsDevMode(builder);
        Assert.False(isDev);
    }
}

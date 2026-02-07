// <copyright file="ServiceCollectionExtensionsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Tests;

using Defra.Identity.Requests.Middleware;
using Defra.Identity.Requests.Users.Commands.Create;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddRequests_Registers_Services_And_Validators()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "DefraIndentityApiKey", "test-api-key" }
            })
            .Build();

        // Act
        services.AddRequests(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        Defra.Identity.Requests.ServiceCollectionExtensions.ApiKey.ShouldBe("test-api-key");
        serviceProvider.GetService<ApiKeyValidationMiddleware>().ShouldNotBeNull();
        serviceProvider.GetService<CorrellationIdMiddleware>().ShouldNotBeNull();
        serviceProvider.GetService<OperatorIdMiddleware>().ShouldNotBeNull();

        // Check if validators are registered
        serviceProvider.GetService<IValidator<CreateUser>>().ShouldNotBeNull();
    }

    [Fact]
    public void UseRequests_Adds_Middlewares_To_Pipeline()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddRequests(builder.Configuration);
        var app = builder.Build();

        // Act & Assert
        Should.NotThrow(() => app.UseRequests());
    }
}

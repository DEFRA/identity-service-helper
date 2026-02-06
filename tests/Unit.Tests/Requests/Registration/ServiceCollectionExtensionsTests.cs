// <copyright file="ServiceCollectionExtensionsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Unit.Tests.Requests.Registration;

using Defra.Identity.Models.Requests.Users.Commands.Create;
using Defra.Identity.Requests.Middleware;
using Defra.Identity.Requests.Registration;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceCollectionExtensions = Defra.Identity.Requests.Registration.ServiceCollectionExtensions;

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
                { "DefraIdentityApiKey", "test-api-key" }
            })
            .Build();

        // Act
        services.AddRequests(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        ServiceCollectionExtensions.ApiKey.ShouldBe("test-api-key");
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

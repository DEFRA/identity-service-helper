// <copyright file="Program.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

using Defra.Identity.Api.Utility.Http;
using Defra.Identity.Api.Utility.Logging;

namespace Defra.Identity.Api;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Defra.Identity.Api.Endpoints.Applications;
using Defra.Identity.Api.Endpoints.Cphs;
using Defra.Identity.Api.Endpoints.Delegations;
using Defra.Identity.Api.Endpoints.Users;
using Defra.Identity.Api.Exceptions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Repositories;
using Defra.Identity.Requests;
using Defra.Identity.Services;
using FluentValidation;
using Serilog;

public class Program
{
    public static async Task Main(string[] args)
    {
        var app = CreateWebApplication(args);
        await app.RunAsync();
        return;
    }

    [ExcludeFromCodeCoverage]
    private static WebApplication CreateWebApplication(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var configuration = builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile(
                $"appsettings.{builder.Environment.EnvironmentName}.json",
                optional: true,
                reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args);

        ConfigureBuilder(builder, configuration.Build());

        var app = builder.Build();
        return SetupApplication(app);
    }

    [ExcludeFromCodeCoverage]
    private static void ConfigureBuilder(
        WebApplicationBuilder builder,
        IConfigurationRoot configuration)
    {
        // Configure logging to use the CDP Platform standards.
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHealthChecks();
        builder.Host.UseSerilog(CdpLogging.Configuration);
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<ApiExceptionHandler>();
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
            options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
        });

        // Default HTTP Client
        builder.Services
            .AddHttpClient("DefaultClient")
            .AddHeaderPropagation();

        // Proxy HTTP Client
        builder.Services.AddTransient<ProxyHttpMessageHandler>();
        builder.Services
            .AddHttpClient("proxy")
            .ConfigurePrimaryHttpMessageHandler<ProxyHttpMessageHandler>();

        // Propagate trace header.
        builder.Services.AddHeaderPropagation(options =>
        {
            var traceHeader = builder.Configuration.GetValue<string>("TraceHeader");
            if (!string.IsNullOrWhiteSpace(traceHeader))
            {
                options.Headers.Add(traceHeader);
            }
        });

        // Add custom services
        builder.Services
            .AddPostgresDatabase(configuration);

        // Add support services
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();
        builder.Services.AddRequests(configuration);

        // Set up the endpoints and their dependencies
        builder.Services.AddRepositories(configuration);
        builder.Services.AddDataServices(configuration);
    }

    [ExcludeFromCodeCoverage]
    private static WebApplication SetupApplication(WebApplication app)
    {
        app.UseSerilogRequestLogging();
        app.UseHeaderPropagation();
        app.UseExceptionHandler();
        app.UseRouting();
        app.UseRequests();
        app.MapHealthChecks("/healthz");
        app.UseUsersEndpoints();
        app.UseApplicationEndpoints();
        app.UseDelegationEndpoints();
        app.UseCphEndpoints();

        return app;
    }
}

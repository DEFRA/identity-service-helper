// <copyright file="Program.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api;

using System.Diagnostics.CodeAnalysis;
using Defra.Identity.Api.Endpoints.Users;
using Defra.Identity.Api.Utils.Http;
using Defra.Identity.Api.Utils.Logging;
using Defra.Identity.Config;
using Defra.Identity.Extensions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
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
        builder.Host.UseSerilog(CdpLogging.Configuration);

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

        // Add AWS defaults
        builder.Services
            .Configure<AwsConfiguration>(builder.Configuration.GetSection("AWS"))
            .AddDefaultAWSOptions(configuration.GetAWSOptions("AWS"));

        // Add custom services
        builder.Services
            .AddAuthDatabase(configuration)
            .AddPollingProcessorService(configuration)
            .AddMessageProcessorService(configuration);

        // Add support services
        builder.Services.AddHealthChecks();
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();

        // Set up the endpoints and their dependencies
        builder.Services.AddTransient<IRepository<UserAccount>, UsersRepository>();
    }

    [ExcludeFromCodeCoverage]
    private static WebApplication SetupApplication(WebApplication app)
    {
        app.UseHeaderPropagation();
        app.UseRouting();
        app.MapHealthChecks("/health");
        app.UseUsersEndpoints();
        //app.UseAuthDatabase();

        return app;
    }
}

using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Livestock.Auth.Api.Endpoints.Users;
using Livestock.Auth.Config;
using Livestock.Auth.Database;
using Livestock.Auth.Database.Entities;
using Livestock.Auth.Extensions;
using Livestock.Auth.Services;
using Livestock.Auth.Utils.Http;
using Livestock.Auth.Utils.Logging;
using Livestock.Auth.Utils.Mongo;
using Serilog;

namespace Livestock.Auth.Api;

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
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
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
        builder.Services.AddAuthDatabase(builder.Configuration);
        // Propagate trace header.
        builder.Services.AddHeaderPropagation(options =>
        {
            var traceHeader = builder.Configuration.GetValue<string>("TraceHeader");
            if (!string.IsNullOrWhiteSpace(traceHeader))
            {
                options.Headers.Add(traceHeader);
            }
        });


        // Set up the MongoDB client. Config and credentials are injected automatically at runtime.
        builder.Services.Configure<MongoConfig>(builder.Configuration.GetSection("Mongo"));
        builder.Services.AddSingleton<IMongoDbClientFactory, MongoDbClientFactory>();

        builder.Services.AddQuartzServices(configuration);
        builder.Services.AddHealthChecks();
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();


        // Set up the endpoints and their dependencies

        builder.Services.AddTransient<IRepository<UserAccount>, UsersRepository>(service =>
            new UsersRepository(service.GetRequiredService<AuthContext>()));
    }

    [ExcludeFromCodeCoverage]
    private static WebApplication SetupApplication(WebApplication app)
    {
        app.UseHeaderPropagation();
        app.UseRouting();
        app.MapHealthChecks("/health");
        app.UseAuthDatabase();
        app.UseUsersEndpoints();

        return app;
    }
}

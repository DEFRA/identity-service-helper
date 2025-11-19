using Livestock.Auth.Database.Tests.Fixtures;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Livestock.Auth.Database.Tests;

using Collections;

[Collection(nameof(PostgreCollection))]
public abstract class BaseTests(PostgreContainerFixture fixture) : IAsyncLifetime
{

    protected AuthContext Context { get; private set; } = null!;

    public async ValueTask DisposeAsync()
    {
        await Context.DisposeAsync();
    }

    public async ValueTask InitializeAsync()
    {
        await fixture.InitializeAsync();
        var builder = WebApplication
            .CreateBuilder();
        builder.Configuration.AddInMemoryCollection(ConnectionStringConfiguration!).Build();
        builder.Services.AddAuthDatabase(builder.Configuration);

        var app = builder.Build();

        app.UseAuthDatabase();
        var serviceProvider = builder.Services.BuildServiceProvider();
        Context =  serviceProvider.GetRequiredService<AuthContext>();
    }

    private Dictionary<string, string> ConnectionStringConfiguration => new()
    {
        { $"ConnectionStrings:{DatabaseConstants.ConnectionStringName}", $"{fixture.ConnectionString}" },
        { "Deployment:Environment", "Dev" },

    };
}

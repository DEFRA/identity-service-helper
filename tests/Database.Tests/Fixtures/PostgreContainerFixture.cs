
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Livestock.Auth.Database.Tests.Fixtures;

public class PostgreContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer container = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .Build();

    public string ConnectionString => container.GetConnectionString();
    public async ValueTask DisposeAsync()
    {
        await container.StopAsync();
    }

    public async ValueTask InitializeAsync()
    {
        await container.StartAsync();
        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseNpgsql(container.GetConnectionString())
            .Options;

        var context = new AuthContext(options);
        await context.Database.MigrateAsync();
    }
}

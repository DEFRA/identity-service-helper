
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace Livestock.Auth.Database.Tests.Fixtures;

public class PostgreContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .Build();
    
    public string ConnectionString => _container.GetConnectionString();
    public async ValueTask DisposeAsync()
    {
        await _container.StopAsync();
    }

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();
        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;
    
        var context = new AuthContext(options);
        await context.Database.MigrateAsync();
    }
}
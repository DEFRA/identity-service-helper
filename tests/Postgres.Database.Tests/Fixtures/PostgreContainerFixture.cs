// <copyright file="PostgreContainerFixture.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Fixtures;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Networks;
using Testcontainers.PostgreSql;

public class PostgreContainerFixture
    : IAsyncLifetime
{
    private INetwork Network { get; set; } = default!;

    private PostgreSqlContainer Db { get; set; } = default!;

    public string ConnectionString => Db.GetConnectionString();

    public async ValueTask DisposeAsync()
    {
        await Db.StopAsync();
        await Network.DeleteAsync();
    }

    public async ValueTask InitializeAsync()
    {
        Network = new NetworkBuilder().Build();

        Db = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithDatabase("appdb")
            .WithUsername("identity_service_helper_ddl")
            .WithPassword("app")
            .WithNetwork(Network)
            .WithNetworkAliases("pg")
            .Build();
        await Db.StartAsync();

        var liquibase = new ContainerBuilder()
            .WithImage("liquibase/liquibase:5.0.1")
            .WithNetwork(Network)
            .WithBindMount(
                Path.GetFullPath("changelog"),   // folder containing master changelog + scripts
                "/liquibase/changelog",
                AccessMode.ReadOnly)
            .WithEntrypoint("sh", "-lc")
            .WithCommand("tail -f /dev/null") // keep container alive
            .Build();
        await liquibase.StartAsync();

        var install = await liquibase.ExecAsync(new[]
        {
            "liquibase",
            "lpm",
            "add",
            "postgresql",
        });
        if (install.ExitCode != 0)
        {
            throw new Exception($"lpm failed: {install.Stderr}");
        }

        var update = await liquibase.ExecAsync(new[]
        {
            "liquibase",
            "--url=jdbc:postgresql://pg:5432/appdb",
            "--username=identity_service_helper_ddl",
            "--password=app",
            "--search-path=/liquibase/changelog",
            "--changelog-file=db.changelog.xml",
            "update",
            "--context-filter=TESTCONTAINER",
        });
        if (update.ExitCode != 0)
        {
            throw new Exception($"liquibase update failed: {update.Stderr}");
        }

        await liquibase.DisposeAsync();
    }
}

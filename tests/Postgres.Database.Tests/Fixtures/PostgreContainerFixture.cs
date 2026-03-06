// <copyright file="PostgreContainerFixture.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

// ReSharper disable ClassNeverInstantiated.Global
namespace Defra.Identity.Postgres.Database.Tests.Fixtures;

using Defra.Identity.Test.Utilities.Database;
using Defra.Identity.Test.Utilities.Locking;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Testcontainers.PostgreSql;

public class PostgreContainerFixture
    : IAsyncLifetime
{
    private static readonly INetwork Network = new NetworkBuilder()
        .Build();

    private static readonly PostgreSqlContainer Db = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithDatabase("appdb")
        .WithUsername("identity_service_helper_ddl")
        .WithPassword("app")
        .WithNetwork(Network)
        .WithNetworkAliases("pg")
        .Build();

    private static readonly IContainer Liquibase = new ContainerBuilder()
        .WithImage("liquibase/liquibase:5.0.1")
        .WithNetwork(Network)
        .WithBindMount(
            Path.GetFullPath("changelog"), // folder containing master changelog + scripts
            "/liquibase/changelog",
            AccessMode.ReadOnly)
        .WithEntrypoint("sh", "-lc")
        .WithCommand("tail -f /dev/null") // keep container alive
        .Build();

    private readonly OnceExecutor liquibaseStartExecutor = new OnceExecutor();
    private readonly OnceExecutor liquibaseUpdateExecutor = new OnceExecutor();

    public static string ConnectionString => Db.GetConnectionString();

    public async ValueTask DisposeAsync()
    {
        await Db.StopAsync();
        await Liquibase.StopAsync();
    }

    public async ValueTask InitializeAsync()
    {
        await liquibaseStartExecutor.ExecuteOnce(StartLiquibase);
        await liquibaseUpdateExecutor.ExecuteOnce(UpdateLiquibase);
        await ResetData();
    }

    private static async Task StartLiquibase()
    {
        await Db.StartAsync();
        await Liquibase.StartAsync();

        var install = await Liquibase.ExecAsync(
        [
            "liquibase", "lpm", "add", "postgresql",
        ]);

        if (install.ExitCode != 0)
        {
            throw new Exception($"lpm failed: {install.Stderr}");
        }
    }

    private static async Task UpdateLiquibase()
    {
        var update = await Liquibase.ExecAsync(
        [
            "liquibase", "--url=jdbc:postgresql://pg:5432/appdb", "--username=identity_service_helper_ddl", "--password=app", "--search-path=/liquibase/changelog",
            "--changelog-file=db.changelog.xml", "update", "--context-filter=TESTCONTAINER",
        ]);

        if (update.ExitCode != 0)
        {
            throw new Exception($"liquibase update failed: {update.Stderr}");
        }
    }

    private static async Task ResetData()
    {
        await SqlHelper.ExecuteSqlFile(ConnectionString, "../../../../../changelog/schema/testcontainer.postgresql.sql");
    }
}

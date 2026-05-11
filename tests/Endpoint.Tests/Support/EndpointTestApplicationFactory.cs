// <copyright file="EndpointTestApplicationFactory.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Endpoint.Tests.Support;

using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

public class EndpointTestApplicationFactory : WebApplicationFactory<Defra.Identity.Api.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            var cs = PostgreContainerFixture.ConnectionString;

            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:PostgresConnection"] = cs,
                ["ConnectionStrings:ReadOnlyPostgresConnection"] = cs,
            });
        });
    }
}

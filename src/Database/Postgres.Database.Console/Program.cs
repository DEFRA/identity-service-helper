// <copyright file="Program.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

using Defra.Identity.Postgres.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    EnvironmentName = Environments.Development,
    ContentRootPath = AppContext.BaseDirectory,
});

builder.Configuration.SetBasePath(AppContext.BaseDirectory);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

builder.Host.UseDefaultServiceProvider(o =>
{
    o.ValidateScopes = false;
    o.ValidateOnBuild = false;
});

builder.Services.AddPostgresDatabase(builder.Configuration);

var app = builder.Build();

Console.WriteLine($"Running in {app.Environment.EnvironmentName} mode.");

app.UseDatabaseMigrations();

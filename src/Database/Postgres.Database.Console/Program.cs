// <copyright file="Program.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

using Defra.Identity.Postgres.Database;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthDatabase(builder.Configuration);
var app = builder.Build();
app.UseDatabaseMigrations();

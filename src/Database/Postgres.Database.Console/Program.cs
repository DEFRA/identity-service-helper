// See https://aka.ms/new-console-template for more information

using System;
using Defra.Identity.Postgres.Database;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthDatabase(builder.Configuration);
var app = builder.Build();
app.UseDatabaseMigrations();

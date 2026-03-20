// <copyright file="Program.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.DataSeeder;

using System.CommandLine;
using Defra.Identity.DataSeeder.Commands;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        RootCommand rootCommand = new("Database utilities for Defra Identity.");
        rootCommand.Add(new RunScriptCommand());

        ParseResult parseResult = rootCommand.Parse(args);
        return await parseResult.InvokeAsync();
    }
}

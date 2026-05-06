// <copyright file="RootCommandTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.DataSeeder.Tests;

using System.CommandLine;
using Defra.Identity.DataSeeder.Commands;
using Shouldly;

public class RootCommandTests
{
    [Fact]
    public void RootCommand_ShouldContainRunScriptCommand()
    {
        // Arrange
        var rootCommand = new RootCommand("Database utilities for Defra Identity.");
        rootCommand.Add(new TestCommand());

        // Act
        var subcommands = rootCommand.Subcommands.ToList();

        // Assert
        subcommands.Count.ShouldBe(1);
        subcommands.ShouldContain(c => c.Name == "TestCommand");
    }

    public class TestCommand()
        : BaseCommand("TestCommand", "My test command");
}

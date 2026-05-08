// <copyright file="RunScriptCommandTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.DataSeeder.Tests;

using System.CommandLine;
using Defra.Identity.DataSeeder.Commands;
using Shouldly;

public class RunScriptCommandTests
{
    [Fact]
    public void RunScriptCommand_ShouldHaveRunAlias()
    {
        // Arrange
        var command = new RunScriptCommand();

        // Act
        var aliases = command.Aliases.ToList();

        // Assert
        aliases.ShouldContain("run");
    }

    [Fact]
    public void RunScriptCommand_ShouldHaveRequiredOptions()
    {
        // Arrange
        var command = new RunScriptCommand();

        // Act
        var options = command.Options.ToList();

        // Assert
        options.Count.ShouldBe(4);
        options.ShouldContain(o => o.Name == "-db" && o.Required);
        options.ShouldContain(o => o.Name == "-uid" && o.Required);
        options.ShouldContain(o => o.Name == "-pwd" && o.Required);
        options.ShouldContain(o => o.Name == "-script" && o.Required);
    }

    [Fact]
    public void RunScriptCommand_ShouldFailValidation_WhenScriptFileDoesNotExist()
    {
        // Arrange
        var rootCommand = new RootCommand();
        rootCommand.Add(new RunScriptCommand());
        var args = new[] { "run", "-db", "test", "-uid", "user", "-pwd", "pass", "-script", "nonexistent.sql" };

        // Act
        var parseResult = rootCommand.Parse(args);

        // Assert
        parseResult.Errors.ShouldNotBeEmpty();
        parseResult.Errors[0].Message.ShouldContain("Script file does not exist");
    }

    [Fact]
    public void RunScriptCommand_ShouldParseSuccessfully_WithAllRequiredArguments()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            var rootCommand = new RootCommand();
            rootCommand.Add(new RunScriptCommand());
            var args = new[] { "run", "-db", "postgresql://localhost/testdb", "-uid", "testuser", "-pwd", "testpass", "-script", tempFile };

            // Act
            var parseResult = rootCommand.Parse(args);

            // Assert
            parseResult.Errors.ShouldBeEmpty();
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public void RunScriptCommand_Invoke_Returns_Error()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            var rootCommand = new RootCommand();
            rootCommand.Add(new RunScriptCommand());
            var args = new[] { "run", "-db", "postgresql://localhost/testdb", "-uid", "testuser", "-pwd", "testpass", "-script", tempFile };

            // Act
            var parseResult = rootCommand.Parse(args);
            var commandResult = parseResult
                .Invoke();

            // Assert
            parseResult.Errors.ShouldBeEmpty();
            commandResult.ShouldBe(1);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }
}

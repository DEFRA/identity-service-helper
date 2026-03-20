// <copyright file="RunScriptDatabase.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.DataSeeder.Commands;

using System.CommandLine;

/// <summary>
/// Command to run a script on the specified database.
/// </summary>
public class RunScriptCommand : BaseCommand
{
    public RunScriptCommand()
        : base("RunScriptDatabase", "Run a script on the specified database.")
    {
        this.Aliases.Add("run");

        Option<string> scriptFileOption = new("-script")
        {
            Description = "Path to the script file to run.", Required = true,
        };
        scriptFileOption.Validators.Add(result =>
        {
            if (!File.Exists(result.GetValue(scriptFileOption)))
            {
                result.AddError("Script file does not exist");
            }
        });

        this.Add(DatabaseUrlOption);
        this.Add(DatabaseUserNameOption);
        this.Add(DatabasePasswordOption);
        this.Add(scriptFileOption);
        this.SetAction((parsedResult, cancellationToken) => ExecuteAsync(
            parsedResult.GetValue(DatabaseUrlOption),
            parsedResult.GetValue(DatabaseUserNameOption),
            parsedResult.GetValue(DatabasePasswordOption),
            parsedResult.GetValue(scriptFileOption),
            cancellationToken));
    }

    private async Task<int> ExecuteAsync(
        string? databaseUrl,
        string? databaseUserName,
        string? databasePassword,
        string? scriptFile,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(scriptFile);

        if (!File.Exists(scriptFile))
        {
            Console.Error.WriteLine($"The script file '{scriptFile}' could not be found.");
            return 1;
        }

        var connectionString = BuildConnectionString(databaseUrl, databaseUserName, databasePassword);
        var sql = await File.ReadAllTextAsync(scriptFile, cancellationToken);
        try
        {
             await ExecuteNonQueryAsync(connectionString, sql, cancellationToken);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Error running script: {e.Message}");
            return 1;
        }

        Console.WriteLine("Script run complete.");
        return 0;
    }
}

// <copyright file="QueueOptions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.MessageProcessor.Config;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class QueueOptions
{
    public required string Url { get; set; }

    public int WaitTimeSeconds { get; set; } = 20;

    public int MaxNumberOfMessages { get; set; } = 1;

    public List<string> SupportedMessageTypes { get; set; } = new();
}

// <copyright file="QueueOptions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Livestock.Auth.Services.Messaging.Config;

public class QueueOptions
{
    public required string Url { get; set; }

    public int WaitTimeSeconds { get; set; } = 20;

    public int MaxNumberOfMessages { get; set; } = 1;

    public List<string> SupportedMessageTypes { get; set; } = new ();
}

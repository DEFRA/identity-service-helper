// <copyright file="IntakeQueueOptions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Configuration;

public class IntakeQueueOptions
{
    public string Url { get; set; }

    public int WaitTimeSeconds { get; set; } = 20;

    public int MaxNumberOfMessages { get; set; } = 1;

    public List<string> SupportedMessageTypes { get; set; } = new();
}

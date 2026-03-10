// <copyright file="QueueOptions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Configuration;

public class QueueOptions
{
    public IntakeQueueOptions IntakeQueueOptions { get; set; } = new();
}

// <copyright file="ProcessResult.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Messaging.Models;

public class ProcessResult
{
    public ResultGroup Success { get; } = new();

    public ResultGroup Error { get; } = new();

    public class ResultGroup
    {
        public int EmailCountProcessed { get; set; }

        public int SmsCountProcessed { get; set; }
    }
}

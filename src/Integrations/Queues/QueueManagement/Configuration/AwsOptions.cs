// <copyright file="AwsOptions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.QueueManagement.Configuration;

public class AwsOptions
{
    public bool UseLocalStack { get; set; }

    public string ServiceUrl { get; set; } = string.Empty;

    public string Region { get; set; } = string.Empty;

    public string AccessKey { get; set; } = string.Empty;

    public string SecretKey { get; set; } = string.Empty;
}

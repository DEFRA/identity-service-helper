// <copyright file="AwsConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Livestock.Auth.Services.Config;

public class AwsConfiguration
{
    public required bool UseLocalStack { get; init; }

    public string? ServiceURL { get; set; }

    public string? Region { get; set; }

    public string? AccessKey { get; set; }

    public string? SecretKey { get; set; }
}
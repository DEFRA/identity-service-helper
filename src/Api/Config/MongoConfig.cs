// <copyright file="MongoConfig.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Livestock.Auth.Api.Config;

public class MongoConfig
{
    public string DatabaseUri { get; init; } = default!;

    public string DatabaseName { get; init; } = default!;
}

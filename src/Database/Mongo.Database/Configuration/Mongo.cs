// <copyright file="Mongo.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Mongo.Database.Configuration;

public class Mongo
{
    public string DatabaseUri { get; init; } = null!;

    public string DatabaseName { get; init; } = null!;
}

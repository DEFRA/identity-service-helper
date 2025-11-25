// <copyright file="MongoDatabase.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Mongo.Database.Configuration;

public class MongoDatabase
{
    public string Uri { get; init; } = null!;

    public string Name { get; init; } = null!;
}

// <copyright file="IClientFactory.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Mongo.Database;

using MongoDB.Driver;

public interface IClientFactory
{
    IMongoClient Get { get; }

    IMongoCollection<T> Collection<T>(string collection);
}

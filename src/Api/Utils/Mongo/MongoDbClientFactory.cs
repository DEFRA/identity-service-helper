// <copyright file="MongoDbClientFactory.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Utils.Mongo;

using System.Diagnostics.CodeAnalysis;
using Defra.Identity.Api.Config;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

[ExcludeFromCodeCoverage]
public class MongoDbClientFactory : IMongoDbClientFactory
{
    private readonly IMongoDatabase mongoDatabase;
    private readonly MongoClient client;

    public MongoDbClientFactory(IOptions<MongoConfig> config)
    {
        var uri = config.Value.DatabaseUri;
        var databaseName = config.Value.DatabaseName;

        if (string.IsNullOrWhiteSpace(uri))
        {
            throw new ArgumentException("MongoDB uri string cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new ArgumentException("MongoDB database name cannot be empty");
        }

        var settings = MongoClientSettings.FromConnectionString(uri);
        client = new MongoClient(settings);

        var camelCaseConvention = new ConventionPack { new CamelCaseElementNameConvention() };

        // convention must be registered before initialising collection
        ConventionRegistry.Register("CamelCase", camelCaseConvention, _ => true);

        mongoDatabase = client.GetDatabase(databaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string collection)
    {
        return mongoDatabase.GetCollection<T>(collection);
    }

    public IMongoClient GetClient()
    {
        return client;
    }
}

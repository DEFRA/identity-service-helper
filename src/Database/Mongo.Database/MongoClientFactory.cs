// <copyright file="MongoClientFactory.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Mongo.Database;

using Defra.Identity.Mongo.Database.Configuration;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

public class MongoClientFactory : IClientFactory
{
    private readonly IMongoDatabase mongoDatabase;
    private readonly MongoClient client;

    public MongoClientFactory(Mongo config)
    {
        if (config is null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        if (string.IsNullOrWhiteSpace(config.DatabaseUri))
        {
            throw new ArgumentException("MongoDB uri string cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(config.DatabaseName))
        {
            throw new ArgumentException("MongoDB database name cannot be empty");
        }

        var settings = MongoClientSettings.FromConnectionString(config.DatabaseUri);
        client = new MongoClient(settings);

        var camelCaseConvention = new ConventionPack { new CamelCaseElementNameConvention() };

        // convention must be registered before initialising collection
        ConventionRegistry.Register("CamelCase", camelCaseConvention, _ => true);

        mongoDatabase = client.GetDatabase(config.DatabaseName);
    }

    public IMongoClient Get => client;

    public IMongoCollection<T> Collection<T>(string collection)
    {
        return mongoDatabase.GetCollection<T>(collection);
    }
}

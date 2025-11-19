// <copyright file="MongoService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Livestock.Auth.Api.Utils.Mongo;

using System.Diagnostics.CodeAnalysis;
using MongoDB.Driver;

[ExcludeFromCodeCoverage]
public abstract class MongoService<T>
{
    protected MongoService(IMongoDbClientFactory connectionFactory, string collectionName, ILoggerFactory loggerFactory)
    {
        Client = connectionFactory.GetClient();
        Collection = connectionFactory.GetCollection<T>(collectionName);
        var loggerName = GetType().FullName ?? GetType().Name;
        Logger = loggerFactory.CreateLogger(loggerName);
        EnsureIndexes();
    }

    protected IMongoClient Client { get; }

    protected IMongoCollection<T> Collection { get; }

    protected ILogger Logger { get; }

    protected abstract List<CreateIndexModel<T>> DefineIndexes(IndexKeysDefinitionBuilder<T> builder);

    protected void EnsureIndexes()
    {
        var builder = Builders<T>.IndexKeys;
        var indexes = DefineIndexes(builder);
        if (indexes.Count == 0)
        {
            return;
        }

        Logger.LogInformation(
            "Ensuring index is created if it does not exist for collection {CollectionNamespaceCollectionName} in DB {DatabaseDatabaseNamespace}",
            Collection.CollectionNamespace.CollectionName,
            Collection.Database.DatabaseNamespace);
        Collection.Indexes.CreateMany(indexes);
    }
}

// <copyright file="MongoService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Mongo.Database;

using Microsoft.Extensions.Logging;
using MongoDB.Driver;

public abstract class MongoService<TEntity>
    where TEntity : class
{
    protected MongoService(IClientFactory clientFactory, string collectionName, ILoggerFactory loggerFactory)
    {
        Logger = loggerFactory.CreateLogger(GetType().FullName ?? GetType().Name);
        Client = clientFactory.Get;
        Collection = clientFactory.Collection<TEntity>(collectionName);
    }

    protected IMongoClient Client { get; }

    protected IMongoCollection<TEntity> Collection { get; }

    protected ILogger Logger { get; }

    protected abstract List<CreateIndexModel<TEntity>> DefineIndexes(IndexKeysDefinitionBuilder<TEntity> builder);

    protected void EnsureIndexes()
    {
        var builder = Builders<TEntity>.IndexKeys;
        var indexes = DefineIndexes(builder);
        //Collection.Indexes.CreateMany(indexes);
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

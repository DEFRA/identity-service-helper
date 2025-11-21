// <copyright file="MongoServiceTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Unit.Tests.Test.Utils.Mongo;

using System.Collections.Generic;
using Defra.Identity.Api.Utils.Mongo;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Driver;
using NSubstitute;

public class MongoServiceTests
{
    private readonly IMongoDbClientFactory connectionFactoryMock;
    private readonly ILoggerFactory loggerFactoryMock;
    private readonly IMongoClient clientMock;
    private readonly IMongoCollection<TestModel> collectionMock;

    private readonly TestMongoService service;

    public MongoServiceTests()
    {
        connectionFactoryMock = Substitute.For<IMongoDbClientFactory>();
        loggerFactoryMock = Substitute.For<ILoggerFactory>();
        clientMock = Substitute.For<IMongoClient>();
        collectionMock = Substitute.For<IMongoCollection<TestModel>>();

        connectionFactoryMock
            .GetClient()
            .Returns(Substitute.For<IMongoClient>());

        connectionFactoryMock
            .GetCollection<TestModel>(Arg.Any<string>())
            .Returns(collectionMock);

        collectionMock.CollectionNamespace.Returns(
            new CollectionNamespace("test", "example"));

        collectionMock.Database.DatabaseNamespace.Returns(
            new DatabaseNamespace("test"));

        service = new TestMongoService(
            connectionFactoryMock,
            "testCollection",
            NullLoggerFactory.Instance);

        collectionMock.DidNotReceive().Indexes.CreateMany(Arg.Any<IEnumerable<CreateIndexModel<TestModel>>>());
    }

    [Fact]
    public void EnsureIndexes_CreatesIndexes_WhenIndexesAreDefined()
    {
        var indexes = new List<CreateIndexModel<TestModel>>()
        {
            new(Builders<TestModel>.IndexKeys.Ascending(x => x.Name)),
        };
        service.SetIndexes(indexes);
        service.RunEnsureIndexes();

        collectionMock.Received(1).Indexes.CreateMany(indexes, TestContext.Current.CancellationToken);
    }

    [Fact]
    public void EnsureIndexes_DoesNotCreateIndexes_WhenIndexesAreNotDefined()
    {
        service.SetIndexes(new List<CreateIndexModel<TestModel>>());
        service.RunEnsureIndexes();

        collectionMock.DidNotReceive().Indexes.CreateMany(
            Arg.Any<IEnumerable<CreateIndexModel<TestModel>>>(),
            TestContext.Current.CancellationToken);
    }

    public class TestModel
    {
        public string? Name { get; set; }
    }

    private class TestMongoService : MongoService<TestModel>, ITestMongoService
    {
        private List<CreateIndexModel<TestModel>> indexes = new();

        public TestMongoService(
            IMongoDbClientFactory connectionFactory,
            string collectionName,
            ILoggerFactory loggerFactory)
            : base(connectionFactory, collectionName, loggerFactory)
        {
        }

        public List<CreateIndexModel<TestModel>> GetIndexes()
        {
            return indexes;
        }

        public void SetIndexes(List<CreateIndexModel<TestModel>> indexes)
        {
            this.indexes = indexes;
        }

        public void RunEnsureIndexes()
        {
            EnsureIndexes();
        }

        protected override List<CreateIndexModel<TestModel>> DefineIndexes(
            IndexKeysDefinitionBuilder<TestModel> builder)
        {
            return GetIndexes();
        }
    }
}

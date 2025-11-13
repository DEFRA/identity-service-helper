using System.Collections.Generic;
using Livestock.Auth.Utils.Mongo;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Driver;
using NSubstitute;

namespace Livestock.Auth.Test.Utils.Mongo
{
    public class MongoServiceTests
    {
        private readonly IMongoDbClientFactory _connectionFactoryMock;
        private readonly ILoggerFactory _loggerFactoryMock;
        private readonly IMongoClient _clientMock;
        private readonly IMongoCollection<TestModel> _collectionMock;

        private readonly TestMongoService _service;

        public MongoServiceTests()
        {
            this._connectionFactoryMock = Substitute.For<IMongoDbClientFactory>();
            this._loggerFactoryMock = Substitute.For<ILoggerFactory>();
            this._clientMock = Substitute.For<IMongoClient>();
            this._collectionMock = Substitute.For<IMongoCollection<TestModel>>();

            this._connectionFactoryMock
                .GetClient()
                .Returns(Substitute.For<IMongoClient>());

            this._connectionFactoryMock
                .GetCollection<TestModel>(Arg.Any<string>())
                .Returns(this._collectionMock);

            this._collectionMock.CollectionNamespace.Returns(new CollectionNamespace("test", "example"));
            this._collectionMock.Database.DatabaseNamespace.Returns(new DatabaseNamespace("test"));


            this._service = new TestMongoService(this._connectionFactoryMock, "testCollection", NullLoggerFactory.Instance);

            this._collectionMock.DidNotReceive().Indexes.CreateMany(Arg.Any<IEnumerable<CreateIndexModel<TestModel>>>());
        }

        [Fact]
        public void EnsureIndexes_CreatesIndexes_WhenIndexesAreDefined()
        {
            var indexes = new List<CreateIndexModel<TestModel>>()
            {
                new(Builders<TestModel>.IndexKeys.Ascending(x => x.Name)),
            };
            this._service.SetIndexes(indexes);
            this._service.RunEnsureIndexes();

            this._collectionMock.Received(1).Indexes.CreateMany(indexes);
        }

        [Fact]
        public void EnsureIndexes_DoesNotCreateIndexes_WhenIndexesAreNotDefined()
        {
            this._service.SetIndexes(new List<CreateIndexModel<TestModel>>());
            this._service.RunEnsureIndexes();

            this._collectionMock.DidNotReceive().Indexes.CreateMany(Arg.Any<IEnumerable<CreateIndexModel<TestModel>>>());
        }

        public class TestModel
        {
            public string? Name { get; set; }
        }

        public interface ITestMongoService
        {
            public List<CreateIndexModel<TestModel>> GetIndexes();
            public void SetIndexes(List<CreateIndexModel<TestModel>> indexes);
        }

        private class TestMongoService : MongoService<TestModel>, ITestMongoService
        {
            protected List<CreateIndexModel<TestModel>> Indexes = new();

            public TestMongoService(IMongoDbClientFactory connectionFactory, string collectionName,
                ILoggerFactory loggerFactory)
                : base(connectionFactory, collectionName, loggerFactory)
            {
            }

            public List<CreateIndexModel<TestModel>> GetIndexes()
            {
                return this.Indexes;
            }

            public void SetIndexes(List<CreateIndexModel<TestModel>> indexes)
            {
                this.Indexes = indexes;
            }

            protected override List<CreateIndexModel<TestModel>> DefineIndexes(
                IndexKeysDefinitionBuilder<TestModel> builder)
            {
                if (this.GetIndexes() == null)
                {
                    throw new System.Exception("Indexes not defined");
                }

                return this.GetIndexes();
            }

            public void RunEnsureIndexes()
            {
                base.EnsureIndexes();
            }
        }
    }
}
namespace Livestock.Auth.Unit.Tests.Test.Utils.Mongo
{
    using System.Collections.Generic;
    using Livestock.Auth.Api.Utils.Mongo;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using MongoDB.Driver;
    using NSubstitute;

    public class MongoServiceTests
    {
        private readonly IMongoDbClientFactory _connectionFactoryMock;
        private readonly ILoggerFactory _loggerFactoryMock;
        private readonly IMongoClient _clientMock;
        private readonly IMongoCollection<TestModel> _collectionMock;

        private readonly TestMongoService _service;

        public MongoServiceTests()
        {
            _connectionFactoryMock = Substitute.For<IMongoDbClientFactory>();
            _loggerFactoryMock = Substitute.For<ILoggerFactory>();
            _clientMock = Substitute.For<IMongoClient>();
            _collectionMock = Substitute.For<IMongoCollection<TestModel>>();

            _connectionFactoryMock
                .GetClient()
                .Returns(Substitute.For<IMongoClient>());

            _connectionFactoryMock
                .GetCollection<TestModel>(Arg.Any<string>())
                .Returns(_collectionMock);

            _collectionMock.CollectionNamespace.Returns(new CollectionNamespace("test", "example"));
            _collectionMock.Database.DatabaseNamespace.Returns(new DatabaseNamespace("test"));


            _service = new TestMongoService(_connectionFactoryMock, "testCollection", NullLoggerFactory.Instance);

            _collectionMock.DidNotReceive().Indexes.CreateMany(Arg.Any<IEnumerable<CreateIndexModel<TestModel>>>());
        }

        [Fact]
        public void EnsureIndexes_CreatesIndexes_WhenIndexesAreDefined()
        {
            var indexes = new List<CreateIndexModel<TestModel>>()
            {
                new(Builders<TestModel>.IndexKeys.Ascending(x => x.Name)),
            };
            _service.SetIndexes(indexes);
            _service.RunEnsureIndexes();

            _collectionMock.Received(1).Indexes.CreateMany(indexes, TestContext.Current.CancellationToken);
        }

        [Fact]
        public void EnsureIndexes_DoesNotCreateIndexes_WhenIndexesAreNotDefined()
        {
            _service.SetIndexes(new List<CreateIndexModel<TestModel>>());
            _service.RunEnsureIndexes();

            _collectionMock.DidNotReceive().Indexes.CreateMany(Arg.Any<IEnumerable<CreateIndexModel<TestModel>>>(), TestContext.Current.CancellationToken);
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
                return Indexes;
            }

            public void SetIndexes(List<CreateIndexModel<TestModel>> indexes)
            {
                Indexes = indexes;
            }

            protected override List<CreateIndexModel<TestModel>> DefineIndexes(
                IndexKeysDefinitionBuilder<TestModel> builder)
            {
                if (GetIndexes() == null)
                {
                    throw new Exception("Indexes not defined");
                }

                return GetIndexes();
            }

            public void RunEnsureIndexes()
            {
                EnsureIndexes();
            }
        }
    }
}

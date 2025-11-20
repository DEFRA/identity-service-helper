namespace Defra.Identity.Unit.Tests.Test.Utils.Mongo
{
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

            collectionMock.CollectionNamespace.Returns(new CollectionNamespace("test", "example"));
            collectionMock.Database.DatabaseNamespace.Returns(new DatabaseNamespace("test"));


            service = new TestMongoService(connectionFactoryMock, "testCollection", NullLoggerFactory.Instance);

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

            collectionMock.DidNotReceive().Indexes.CreateMany(Arg.Any<IEnumerable<CreateIndexModel<TestModel>>>(), TestContext.Current.CancellationToken);
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

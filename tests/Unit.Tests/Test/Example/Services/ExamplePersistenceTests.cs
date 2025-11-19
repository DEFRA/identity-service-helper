namespace Livestock.Auth.Unit.Tests.Test.Example.Services;

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Livestock.Auth.Api.Example.Models;
using Livestock.Auth.Api.Example.Services;
using Livestock.Auth.Api.Utils.Mongo;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Bson;
using MongoDB.Driver;
using NSubstitute;
using Shouldly;

public class ExamplePersistenceTests
{
    private readonly IMongoDbClientFactory conFactoryMock = Substitute.For<IMongoDbClientFactory>();
    private readonly IMongoCollection<ExampleModel> collectionMock = Substitute.For<IMongoCollection<ExampleModel>>();
    private readonly IMongoDatabase databaseMock = Substitute.For<IMongoDatabase>();
    private readonly CollectionNamespace collectionNamespace = new("test", "example");

    private readonly ExamplePersistence persistence;

    public ExamplePersistenceTests()
    {
        collectionMock
            .CollectionNamespace
            .Returns(collectionNamespace);
        collectionMock
            .Database
            .Returns(databaseMock);
        databaseMock
            .DatabaseNamespace
            .Returns(new DatabaseNamespace("test"));
        conFactoryMock
            .GetClient()
            .Returns(Substitute.For<IMongoClient>());
        conFactoryMock
            .GetCollection<ExampleModel>("example")
            .Returns(collectionMock);

        persistence = new ExamplePersistence(conFactoryMock, NullLoggerFactory.Instance);
    }

    [Fact]
    public async Task CreateAsyncOk()
    {
        collectionMock
            .InsertOneAsync(Arg.Any<ExampleModel>(), cancellationToken: TestContext.Current.CancellationToken)
            .Returns(Task.CompletedTask);

      var example = new ExampleModel
      {
         Id = new ObjectId(),
         Value = "some value",
         Name = "Test",
         Counter = 0
      };
      var result = await persistence.CreateAsync(example);
      result.Should().BeTrue();
   }

    [Fact]
    public async Task CreateAsyncLogError()
    {
        var loggerFactoryMock = Substitute.For<ILoggerFactory>();
        var logMock = Substitute.For<ILogger<ExamplePersistence>>();
        loggerFactoryMock.CreateLogger<ExamplePersistence>().Returns(logMock);

        collectionMock
            .InsertOneAsync(
                Arg.Any<ExampleModel>())
            .Returns(Task.FromException<ExampleModel>(new Exception()));

        var persistence = new ExamplePersistence(conFactoryMock, loggerFactoryMock);

      var example = new ExampleModel()
      {
         Id = new ObjectId(),
         Value = "some value",
         Name = "Test",
         Counter = 0
      };

        var result = await persistence.CreateAsync(example);

        result.Should().BeFalse();
    }
}

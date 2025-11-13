using System;
using System.Threading.Tasks;
using Livestock.Auth.Example.Models;
using Livestock.Auth.Example.Services;
using Livestock.Auth.Utils.Mongo;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Bson;
using MongoDB.Driver;
using NSubstitute;
using FluentAssertions;

namespace Livestock.Auth.Test.Example.Services;

public class ExamplePersistenceTests
{

   private readonly IMongoDbClientFactory _conFactoryMock = Substitute.For<IMongoDbClientFactory>();
   private readonly IMongoCollection<ExampleModel> _collectionMock = Substitute.For<IMongoCollection<ExampleModel>>();
   private readonly IMongoDatabase _databaseMock = Substitute.For<IMongoDatabase>();
   private readonly CollectionNamespace _collectionNamespace = new("test", "example");

   private readonly ExamplePersistence _persistence;

   public ExamplePersistenceTests()
   {
      this._collectionMock
            .CollectionNamespace
            .Returns(this._collectionNamespace);
      this._collectionMock
            .Database
            .Returns(this._databaseMock);
      this._databaseMock
         .DatabaseNamespace
         .Returns(new DatabaseNamespace("test"));
      this._conFactoryMock
         .GetClient()
         .Returns(Substitute.For<IMongoClient>());
      this._conFactoryMock
         .GetCollection<ExampleModel>("example")
         .Returns(this._collectionMock);

      this._persistence = new ExamplePersistence(this._conFactoryMock, NullLoggerFactory.Instance);
   }

   [Fact]
   public async Task CreateAsyncOk()
   {
      this._collectionMock
          .InsertOneAsync(Arg.Any<ExampleModel>(), cancellationToken: TestContext.Current.CancellationToken)
          .Returns(Task.CompletedTask);

      var example = new ExampleModel
      {
         Id = new ObjectId(),
         Value = "some value",
         Name = "Test",
         Counter = 0
      };
      var result = await this._persistence.CreateAsync(example);
      result.Should().BeTrue();
   }

   [Fact]
   public async Task CreateAsyncLogError()
   {

      var loggerFactoryMock = Substitute.For<ILoggerFactory>();
      var logMock = Substitute.For<ILogger<ExamplePersistence>>();
      loggerFactoryMock.CreateLogger<ExamplePersistence>().Returns(logMock);

      this._collectionMock
          .InsertOneAsync(Arg.Any<ExampleModel>())
          .Returns(Task.FromException<ExampleModel>(new Exception()));

      var persistence = new ExamplePersistence(this._conFactoryMock, loggerFactoryMock);

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

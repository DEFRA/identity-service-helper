// <copyright file="ITestMongoService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Unit.Tests.Test.Utils.Mongo;

using MongoDB.Driver;

public interface ITestMongoService
{
    List<CreateIndexModel<MongoServiceTests.TestModel>> GetIndexes();

    void SetIndexes(List<CreateIndexModel<MongoServiceTests.TestModel>> indexes);
}

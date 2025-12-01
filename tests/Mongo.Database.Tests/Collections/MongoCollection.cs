// <copyright file="MongoCollection.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Mongo.Database.Tests.Collections;

using Defra.Identity.Mongo.Database.Tests.Fixtures;

[CollectionDefinition(nameof(MongoCollection))]
public class MongoCollection : ICollectionFixture<MongoContainerFixture>;

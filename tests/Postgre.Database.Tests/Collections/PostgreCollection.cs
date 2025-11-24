// <copyright file="PostgreCollection.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgre.Database.Tests.Collections;

using Defra.Identity.Postgre.Database.Tests.Fixtures;

[CollectionDefinition(nameof(PostgreCollection))]
public class PostgreCollection : ICollectionFixture<PostgreContainerFixture>;

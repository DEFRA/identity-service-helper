// <copyright file="PostgreCollection.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Database.Tests.Collections;

using Defra.Identity.Database.Tests.Fixtures;

[CollectionDefinition(nameof(PostgreCollection))]
public class PostgreCollection : ICollectionFixture<PostgreContainerFixture>;

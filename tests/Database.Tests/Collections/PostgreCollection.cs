using Livestock.Auth.Database.Tests.Fixtures;

namespace Livestock.Auth.Database.Tests.Collections;

[CollectionDefinition(nameof(PostgreCollection))]
public class PostgreCollection : ICollectionFixture<PostgreContainerFixture>;

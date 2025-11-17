using Livestock.Auth.Database.Tests.Fixtures;
using Xunit;

namespace Livestock.Auth.Database.Tests.Collections;

[CollectionDefinition(nameof(PostgreCollection))]
public class PostgreCollection : ICollectionFixture<PostgreContainerFixture>;

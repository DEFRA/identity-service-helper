// <copyright file="CreateTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.CphRepositoryTests;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Cphs;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class CreateTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Should get paged cphs in ascending order with correct paging details")]
    public Task ShouldThrowNotImplementedException()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphRepository>>();
        var repository = new CphRepository(Context, logger);

        var newEntity = new CountyParishHoldings();

        // Act & Assert
        Should.Throw<NotImplementedException>(async () => await repository.Create(newEntity, TestContext.Current.CancellationToken));
        return Task.CompletedTask;
    }
}

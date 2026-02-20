// <copyright file="UpdateTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.CphRepositoryTests;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Postgres.Database.Tests.Fixtures.SeedData;
using Defra.Identity.Repositories.Cphs;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class UpdateTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Should update an existing user account")]
    public async Task ShouldUpdateEntity()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphRepository>>();
        var repository = new CphRepository(Context, logger);

        var adminUser = await SeedDataQueryHelper.GetAdminUser(Context);
        var id = Guid.NewGuid();
        const string identifier = "44/001/0001";
        var createAtDate = DateTime.Parse("2026-02-20").ToUniversalTime();
        var createdById = adminUser.Id;

        var newEntity = new CountyParishHoldings
        {
            Id = id, Identifier = identifier, CreatedAt = createAtDate, CreatedById = createdById,
        };

        await Context.CountyParishHoldings.AddAsync(newEntity, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var entityToUpdate = await repository.GetSingle(x => x.Identifier == identifier, TestContext.Current.CancellationToken);

        entityToUpdate.ShouldNotBeNull();

        // Act
        entityToUpdate.ExpiredAt = DateTime.Parse("2026-03-21").ToUniversalTime();
        entityToUpdate.DeletedAt = DateTime.Parse("2026-03-22").ToUniversalTime();
        entityToUpdate.DeletedById = adminUser.Id;

        var updatedEntityReturnedFromUpdate = await repository.Update(entityToUpdate, TestContext.Current.CancellationToken);
        var updatedEntityReturnedFromRequery = await repository.GetSingle(x => x.Id == id, TestContext.Current.CancellationToken);

        // Assert
        updatedEntityReturnedFromUpdate.ShouldNotBeNull();
        updatedEntityReturnedFromRequery.ShouldNotBeNull();

        updatedEntityReturnedFromUpdate.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(id),
            (x) => x.Identifier.ShouldBe(identifier),
            (x) => x.CreatedAt.ShouldBe(createAtDate),
            (x) => x.CreatedById.ShouldBe(createdById),
            (x) => x.ExpiredAt.ShouldBe(DateTime.Parse("2026-03-21").ToUniversalTime()),
            (x) => x.DeletedAt.ShouldBe(DateTime.Parse("2026-03-22").ToUniversalTime()),
            (x) => x.DeletedById.ShouldBe(adminUser.Id));

        updatedEntityReturnedFromRequery.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(id),
            (x) => x.Identifier.ShouldBe(identifier),
            (x) => x.CreatedAt.ShouldBe(createAtDate),
            (x) => x.CreatedById.ShouldBe(createdById),
            (x) => x.ExpiredAt.ShouldBe(DateTime.Parse("2026-03-21").ToUniversalTime()),
            (x) => x.DeletedAt.ShouldBe(DateTime.Parse("2026-03-22").ToUniversalTime()),
            (x) => x.DeletedById.ShouldBe(adminUser.Id));
    }
}

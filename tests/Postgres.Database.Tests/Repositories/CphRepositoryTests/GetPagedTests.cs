// <copyright file="GetPagedTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.CphRepositoryTests;

using System.ComponentModel;
using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Cphs;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class GetPagedTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Should get paged cphs in ascending order with correct paging details")]
    public async Task ShouldGetPageOneCphsInAscendingOrderWithPageSizeOfThree()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphRepository>>();
        var repository = new CphRepository(Context, ReadOnlyContext, logger);

        Expression<Func<CountyParishHoldings, bool>> filter = cph => true;
        Expression<Func<CountyParishHoldings, string>> orderBy = cph => cph.Identifier;

        const int pageNumber = 1;
        const int pageSize = 3;
        const bool descendingOrder = false;

        // Act
        var pagedEntities = await repository.GetPaged(filter, pageNumber, pageSize, orderBy, descendingOrder, TestContext.Current.CancellationToken);

        // Assert
        logger.Received(1).Log(LogLevel.Information, "Getting list of county parish holdings");

        pagedEntities.ShouldSatisfyAllConditions(
            (x) => x.Items.Count.ShouldBe(3),
            (x) => x.PageSize.ShouldBe(pageSize),
            (x) => x.PageNumber.ShouldBe(pageNumber),
            (x) => x.TotalCount.ShouldBe(11),
            (x) => x.TotalPages.ShouldBe(4));

        var firstItem = pagedEntities.Items[0];
        var secondItem = pagedEntities.Items[1];
        var thirdItem = pagedEntities.Items[2];

        firstItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("4435a146-d0ac-4260-8a27-c550e0ed9563")),
            (x) => x.Identifier.ShouldBe("44/000/0001"),
            (x) => x.CreatedAt.ShouldBe(DateTime.Parse("2026-02-01").ToUniversalTime()),
            (x) => x.CreatedById.ShouldBe(AdminUserId),
            (x) => x.ExpiredAt.ShouldBeNull(),
            (x) => x.DeletedAt.ShouldBeNull(),
            (x) => x.DeletedById.ShouldBeNull());

        secondItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("204459b1-3a07-4e65-9122-91c1699e3d3f")),
            (x) => x.Identifier.ShouldBe("44/000/0002"),
            (x) => x.CreatedAt.ShouldBe(DateTime.Parse("2026-02-02").ToUniversalTime()),
            (x) => x.CreatedById.ShouldBe(AdminUserId),
            (x) => x.ExpiredAt.ShouldBeNull(),
            (x) => x.DeletedAt.ShouldBeNull(),
            (x) => x.DeletedById.ShouldBeNull());

        thirdItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("1eb0f2fb-a332-4cd5-8a20-02d7adfd7156")),
            (x) => x.Identifier.ShouldBe("44/000/0003"),
            (x) => x.CreatedAt.ShouldBe(DateTime.Parse("2026-02-03").ToUniversalTime()),
            (x) => x.CreatedById.ShouldBe(AdminUserId),
            (x) => x.ExpiredAt.ShouldBeNull(),
            (x) => x.DeletedAt.ShouldBeNull(),
            (x) => x.DeletedById.ShouldBeNull());
    }

    [Fact]
    [Description("Should get paged cphs in descending order with correct paging details")]
    public async Task ShouldGetPageOneCphsInDescendingOrderWithPageSizeOfThree()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphRepository>>();
        var repository = new CphRepository(Context, ReadOnlyContext, logger);

        Expression<Func<CountyParishHoldings, bool>> filter = cph => true;
        Expression<Func<CountyParishHoldings, string>> orderBy = cph => cph.Identifier;

        const int pageNumber = 1;
        const int pageSize = 3;
        const bool descendingOrder = true;

        // Act
        var pagedEntities = await repository.GetPaged(filter, pageNumber, pageSize, orderBy, descendingOrder, TestContext.Current.CancellationToken);

        // Assert
        logger.Received(1).Log(LogLevel.Information, "Getting list of county parish holdings");

        pagedEntities.ShouldSatisfyAllConditions(
            (x) => x.Items.Count.ShouldBe(3),
            (x) => x.PageSize.ShouldBe(pageSize),
            (x) => x.PageNumber.ShouldBe(pageNumber),
            (x) => x.TotalCount.ShouldBe(11),
            (x) => x.TotalPages.ShouldBe(4));

        var firstItem = pagedEntities.Items[0];
        var secondItem = pagedEntities.Items[1];
        var thirdItem = pagedEntities.Items[2];

        firstItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("d9a711ec-722d-49b6-abcc-23f0795e3886")),
            (x) => x.Identifier.ShouldBe("44/000/0025"),
            (x) => x.CreatedAt.ShouldBe(DateTime.Parse("2026-01-03").ToUniversalTime()),
            (x) => x.CreatedById.ShouldBe(AdminUserId),
            (x) => x.ExpiredAt.ShouldBe(DateTime.Parse("2026-02-10").ToUniversalTime()),
            (x) => x.DeletedAt.ShouldBeNull(),
            (x) => x.DeletedById.ShouldBeNull());

        secondItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("81e0c45e-8340-4f57-a339-cd52c23372c7")),
            (x) => x.Identifier.ShouldBe("44/000/0024"),
            (x) => x.CreatedAt.ShouldBe(DateTime.Parse("2026-01-03").ToUniversalTime()),
            (x) => x.CreatedById.ShouldBe(AdminUserId),
            (x) => x.ExpiredAt.ShouldBe(DateTime.Parse("2026-02-10").ToUniversalTime()),
            (x) => x.DeletedAt.ShouldBeNull(),
            (x) => x.DeletedById.ShouldBeNull());

        thirdItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("ebc992ae-2b95-4549-9fa3-4484c8349b89")),
            (x) => x.Identifier.ShouldBe("44/000/0023"),
            (x) => x.CreatedAt.ShouldBe(DateTime.Parse("2026-01-03").ToUniversalTime()),
            (x) => x.CreatedById.ShouldBe(AdminUserId),
            (x) => x.ExpiredAt.ShouldBeNull(),
            (x) => x.DeletedAt.ShouldBeNull(),
            (x) => x.DeletedById.ShouldBeNull());
    }
}

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
            (x) => x.TotalCount.ShouldBe(23),
            (x) => x.TotalPages.ShouldBe(8));

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
            (x) => x.TotalCount.ShouldBe(23),
            (x) => x.TotalPages.ShouldBe(8));

        var firstItem = pagedEntities.Items[0];
        var secondItem = pagedEntities.Items[1];
        var thirdItem = pagedEntities.Items[2];

        firstItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("ab820006-0000-4000-8000-000000000006")),
            (x) => x.Identifier.ShouldBe("44/082/0006"),
            (x) => x.CreatedAt.ShouldBe(DateTime.Parse("2026-04-16")),
            (x) => x.CreatedById.ShouldBe(AdminUserId),
            (x) => x.ExpiredAt.ShouldBeNull(),
            (x) => x.DeletedAt.ShouldBeNull(),
            (x) => x.DeletedById.ShouldBeNull());

        secondItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("ab820005-0000-4000-8000-000000000005")),
            (x) => x.Identifier.ShouldBe("44/082/0005"),
            (x) => x.CreatedAt.ShouldBe(DateTime.Parse("2026-04-16")),
            (x) => x.CreatedById.ShouldBe(AdminUserId),
            (x) => x.ExpiredAt.ShouldBeNull(),
            (x) => x.DeletedAt.ShouldBeNull(),
            (x) => x.DeletedById.ShouldBeNull());

        thirdItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("ab820004-0000-4000-8000-000000000004")),
            (x) => x.Identifier.ShouldBe("44/082/0004"),
            (x) => x.CreatedAt.ShouldBe(DateTime.Parse("2026-04-16")),
            (x) => x.CreatedById.ShouldBe(AdminUserId),
            (x) => x.ExpiredAt.ShouldBeNull(),
            (x) => x.DeletedAt.ShouldBeNull(),
            (x) => x.DeletedById.ShouldBeNull());
    }
}

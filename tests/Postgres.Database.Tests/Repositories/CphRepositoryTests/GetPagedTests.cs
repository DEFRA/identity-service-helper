// <copyright file="GetPagedTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.CphRepositoryTests;

using System.ComponentModel;
using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Postgres.Database.Tests.Fixtures.TestData.Helpers;
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
        var repository = new CphRepository(Context, logger);

        Expression<Func<CountyParishHoldings, bool>> filter = cph => true;
        Expression<Func<CountyParishHoldings, string>> orderBy = cph => cph.Identifier;

        const int pageNumber = 1;
        const int pageSize = 3;
        const bool descendingOrder = false;

        var adminUser = await TestDataHelper.GetAdminUser(Context);

        // Act
        var pagedEntities = await repository.GetPaged(filter, pageNumber, pageSize, orderBy, descendingOrder, TestContext.Current.CancellationToken);

        logger.Received(1).Log(LogLevel.Information, "Getting list of county parish holdings");

        pagedEntities.Items.Count.ShouldBe(3);
        pagedEntities.PageSize.ShouldBe(pageSize);
        pagedEntities.PageNumber.ShouldBe(pageNumber);
        pagedEntities.TotalCount.ShouldBe(7);
        pagedEntities.TotalPages.ShouldBe(3);

        var firstItem = pagedEntities.Items[0];
        var secondItem = pagedEntities.Items[1];
        var thirdItem = pagedEntities.Items[2];

        firstItem.Id.ShouldBe(new Guid("4435a146-d0ac-4260-8a27-c550e0ed9563"));
        firstItem.Identifier.ShouldBe("44/000/0001");
        firstItem.CreatedAt.ShouldBe(DateTime.Parse("01/02/2026"));
        firstItem.CreatedById.ShouldBe(adminUser.Id);

        secondItem.Id.ShouldBe(new Guid("204459b1-3a07-4e65-9122-91c1699e3d3f"));
        secondItem.Identifier.ShouldBe("44/000/0002");
        secondItem.CreatedAt.ShouldBe(DateTime.Parse("02/02/2026"));
        secondItem.CreatedById.ShouldBe(adminUser.Id);

        thirdItem.Id.ShouldBe(new Guid("1eb0f2fb-a332-4cd5-8a20-02d7adfd7156"));
        thirdItem.Identifier.ShouldBe("44/000/0003");
        thirdItem.CreatedAt.ShouldBe(DateTime.Parse("03/02/2026"));
        thirdItem.CreatedById.ShouldBe(adminUser.Id);
    }

    [Fact]
    [Description("Should get paged cphs in descending order with correct paging details")]
    public async Task ShouldGetPageOneCphsInDescendingOrderWithPageSizeOfThree()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphRepository>>();
        var repository = new CphRepository(Context, logger);

        Expression<Func<CountyParishHoldings, bool>> filter = cph => true;
        Expression<Func<CountyParishHoldings, string>> orderBy = cph => cph.Identifier;

        const int pageNumber = 1;
        const int pageSize = 3;
        const bool descendingOrder = true;

        var adminUser = await TestDataHelper.GetAdminUser(Context);

        // Act
        var pagedEntities = await repository.GetPaged(filter, pageNumber, pageSize, orderBy, descendingOrder, TestContext.Current.CancellationToken);

        logger.Received(1).Log(LogLevel.Information, "Getting list of county parish holdings");

        pagedEntities.Items.Count.ShouldBe(3);
        pagedEntities.PageSize.ShouldBe(pageSize);
        pagedEntities.PageNumber.ShouldBe(pageNumber);
        pagedEntities.TotalCount.ShouldBe(7);
        pagedEntities.TotalPages.ShouldBe(3);

        var firstItem = pagedEntities.Items[0];
        var secondItem = pagedEntities.Items[1];
        var thirdItem = pagedEntities.Items[2];

        firstItem.Id.ShouldBe(new Guid("088967e7-71b8-457a-9001-5b71f24798fd"));
        firstItem.Identifier.ShouldBe("44/000/0007");
        firstItem.CreatedAt.ShouldBe(DateTime.Parse("07/02/2026"));
        firstItem.CreatedById.ShouldBe(adminUser.Id);

        secondItem.Id.ShouldBe(new Guid("82181a8b-7f7f-470c-9263-2b94675599df"));
        secondItem.Identifier.ShouldBe("44/000/0006");
        secondItem.CreatedAt.ShouldBe(DateTime.Parse("06/02/2026"));
        secondItem.CreatedById.ShouldBe(adminUser.Id);

        thirdItem.Id.ShouldBe(new Guid("7973060a-d483-4ad4-9716-c70415ed620a"));
        thirdItem.Identifier.ShouldBe("44/000/0005");
        thirdItem.CreatedAt.ShouldBe(DateTime.Parse("05/02/2026"));
        thirdItem.CreatedById.ShouldBe(adminUser.Id);
    }
}

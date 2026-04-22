// <copyright file="GetPagedTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.CphUsersRepositoryTests;

using System.ComponentModel;
using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Cphs;
using Defra.Identity.Repositories.Cphs.Users;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class GetPagedTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Should get page one of cph users in ascending order with ordering and filters applied")]
    public async Task ShouldGetPageOneCphUsersInAscendingOrderWithOrderingAndFiltersApplied()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphAssigneesRepository>>();
        var repository = new CphAssigneesRepository(ReadOnlyContext, logger);

        var cphId = new Guid("4435a146-d0ac-4260-8a27-c550e0ed9563");
        const int pageNumber = 1;
        const int pageSize = 2;
        const bool descendingOrder = false;

        Expression<Func<CountyParishHoldings, bool>> primaryFilter = cph => cph.Id == cphId;
        Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> associationFilter = cphUser => cphUser.DeletedAt == null;
        Expression<Func<ApplicationUserAccountHoldingAssignments, string>> orderBy = cphUser => cphUser.UserAccount.DisplayName;

        // Act
        var pagedEntities = await repository.GetPaged(primaryFilter, associationFilter, pageNumber, pageSize, orderBy, descendingOrder, TestContext.Current.CancellationToken);

        // Assert
        logger.Received(1).Log(LogLevel.Information, "Getting list of users for county parish holding");

        pagedEntities.ShouldSatisfyAllConditions(
            (x) => x.Items.Count.ShouldBe(2),
            (x) => x.PageSize.ShouldBe(pageSize),
            (x) => x.PageNumber.ShouldBe(pageNumber),
            (x) => x.TotalCount.ShouldBe(3),
            (x) => x.TotalPages.ShouldBe(2));

        var firstItem = pagedEntities.Items[0];
        var secondItem = pagedEntities.Items[1];

        firstItem.ShouldSatisfyAllConditions(
            (x) => x.ApplicationId.ShouldBe(new Guid("112788f5-4cb5-4acc-a3f5-d8b2b0e20945")),
            (x) => x.RoleId.ShouldBe(new Guid("0c15ba2f-b4ba-406a-a0ae-213de64600a9")),
            (x) => x.UserAccountId.ShouldBe(new Guid("42bde7a0-9efe-402a-a7c3-9161be7b00ba")),
            (x) => x.CreatedAt.ShouldBe(DateTime.Parse("2026-03-01").ToUniversalTime()),
            (x) => x.CreatedById.ShouldBe(AdminUserId),
            (x) => x.DeletedAt.ShouldBeNull(),
            (x) => x.DeletedById.ShouldBeNull());

        secondItem.ShouldSatisfyAllConditions(
            (x) => x.ApplicationId.ShouldBe(new Guid("112788f5-4cb5-4acc-a3f5-d8b2b0e20945")),
            (x) => x.RoleId.ShouldBe(new Guid("817647b3-d5d2-45e9-8833-df36d8264102")),
            (x) => x.UserAccountId.ShouldBe(new Guid("1e21b685-2247-4d96-bf39-f7dc30f356c2")),
            (x) => x.CreatedAt.ShouldBe(DateTime.Parse("2026-03-02").ToUniversalTime()),
            (x) => x.CreatedById.ShouldBe(AdminUserId),
            (x) => x.DeletedAt.ShouldBeNull(),
            (x) => x.DeletedById.ShouldBeNull());
    }

    [Fact]
    [Description("Should get page two of cph users in ascending order with ordering and filters applied")]
    public async Task ShouldGetPageTwoCphUsersInAscendingOrderWithOrderingAndFiltersApplied()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphAssigneesRepository>>();
        var repository = new CphAssigneesRepository(ReadOnlyContext, logger);

        var cphId = new Guid("4435a146-d0ac-4260-8a27-c550e0ed9563");
        const int pageNumber = 2;
        const int pageSize = 2;
        const bool descendingOrder = false;

        Expression<Func<CountyParishHoldings, bool>> primaryFilter = cph => cph.Id == cphId;
        Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> associationFilter = cphUser => cphUser.DeletedAt == null;
        Expression<Func<ApplicationUserAccountHoldingAssignments, string>> orderBy = cphUser => cphUser.UserAccount.DisplayName;

        // Act
        var pagedEntities = await repository.GetPaged(primaryFilter, associationFilter, pageNumber, pageSize, orderBy, descendingOrder, TestContext.Current.CancellationToken);

        // Assert
        logger.Received(1).Log(LogLevel.Information, "Getting list of users for county parish holding");

        pagedEntities.ShouldSatisfyAllConditions(
            (x) => x.Items.Count.ShouldBe(1),
            (x) => x.PageSize.ShouldBe(pageSize),
            (x) => x.PageNumber.ShouldBe(pageNumber),
            (x) => x.TotalCount.ShouldBe(3),
            (x) => x.TotalPages.ShouldBe(2));

        var firstItem = pagedEntities.Items[0];

        firstItem.ShouldSatisfyAllConditions(
            (x) => x.ApplicationId.ShouldBe(new Guid("5466ef9b-aa6b-4b7d-9aac-6c6e55a66ab9")),
            (x) => x.RoleId.ShouldBe(new Guid("306fa0fc-bd1a-45d3-9fef-e6f11a85b601")),
            (x) => x.UserAccountId.ShouldBe(new Guid("d1354eb1-dd1c-471e-bd0e-2626e2e21366")),
            (x) => x.CreatedAt.ShouldBe(DateTime.Parse("2026-03-04").ToUniversalTime()),
            (x) => x.CreatedById.ShouldBe(AdminUserId),
            (x) => x.DeletedAt.ShouldBeNull(),
            (x) => x.DeletedById.ShouldBeNull());
    }

    [Fact]
    [Description("Should get page one of cph users in descending order with ordering and filters applied")]
    public async Task ShouldGetPageOneCphUsersInDescendingOrderWithOrderingAndFiltersApplied()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphAssigneesRepository>>();
        var repository = new CphAssigneesRepository(ReadOnlyContext, logger);

        var cphId = new Guid("4435a146-d0ac-4260-8a27-c550e0ed9563");
        const int pageNumber = 1;
        const int pageSize = 2;
        const bool descendingOrder = true;

        Expression<Func<CountyParishHoldings, bool>> primaryFilter = cph => cph.Id == cphId;
        Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> associationFilter = cphUser => cphUser.DeletedAt == null;
        Expression<Func<ApplicationUserAccountHoldingAssignments, string>> orderBy = cphUser => cphUser.UserAccount.DisplayName;

        // Act
        var pagedEntities = await repository.GetPaged(primaryFilter, associationFilter, pageNumber, pageSize, orderBy, descendingOrder, TestContext.Current.CancellationToken);

        // Assert
        logger.Received(1).Log(LogLevel.Information, "Getting list of users for county parish holding");

        pagedEntities.ShouldSatisfyAllConditions(
            (x) => x.Items.Count.ShouldBe(2),
            (x) => x.PageSize.ShouldBe(pageSize),
            (x) => x.PageNumber.ShouldBe(pageNumber),
            (x) => x.TotalCount.ShouldBe(3),
            (x) => x.TotalPages.ShouldBe(2));

        var firstItem = pagedEntities.Items[0];
        var secondItem = pagedEntities.Items[1];

        firstItem.ShouldSatisfyAllConditions(
            (x) => x.ApplicationId.ShouldBe(new Guid("5466ef9b-aa6b-4b7d-9aac-6c6e55a66ab9")),
            (x) => x.RoleId.ShouldBe(new Guid("306fa0fc-bd1a-45d3-9fef-e6f11a85b601")),
            (x) => x.UserAccountId.ShouldBe(new Guid("d1354eb1-dd1c-471e-bd0e-2626e2e21366")),
            (x) => x.CreatedAt.ShouldBe(DateTime.Parse("2026-03-04").ToUniversalTime()),
            (x) => x.CreatedById.ShouldBe(AdminUserId),
            (x) => x.DeletedAt.ShouldBeNull(),
            (x) => x.DeletedById.ShouldBeNull());

        secondItem.ShouldSatisfyAllConditions(
            (x) => x.ApplicationId.ShouldBe(new Guid("112788f5-4cb5-4acc-a3f5-d8b2b0e20945")),
            (x) => x.RoleId.ShouldBe(new Guid("817647b3-d5d2-45e9-8833-df36d8264102")),
            (x) => x.UserAccountId.ShouldBe(new Guid("1e21b685-2247-4d96-bf39-f7dc30f356c2")),
            (x) => x.CreatedAt.ShouldBe(DateTime.Parse("2026-03-02").ToUniversalTime()),
            (x) => x.CreatedById.ShouldBe(AdminUserId),
            (x) => x.DeletedAt.ShouldBeNull(),
            (x) => x.DeletedById.ShouldBeNull());
    }

    [Fact]
    [Description("Should get page two of cph users in desending order with ordering and filters applied")]
    public async Task ShouldGetPageTwoCphUsersInDescendingOrderWithOrderingAndFiltersApplied()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphAssigneesRepository>>();
        var repository = new CphAssigneesRepository(ReadOnlyContext, logger);

        var cphId = new Guid("4435a146-d0ac-4260-8a27-c550e0ed9563");
        const int pageNumber = 2;
        const int pageSize = 2;
        const bool descendingOrder = true;

        Expression<Func<CountyParishHoldings, bool>> primaryFilter = cph => cph.Id == cphId;
        Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> associationFilter = cphUser => cphUser.DeletedAt == null;
        Expression<Func<ApplicationUserAccountHoldingAssignments, string>> orderBy = cphUser => cphUser.UserAccount.DisplayName;

        // Act
        var pagedEntities = await repository.GetPaged(primaryFilter, associationFilter, pageNumber, pageSize, orderBy, descendingOrder, TestContext.Current.CancellationToken);

        // Assert
        logger.Received(1).Log(LogLevel.Information, "Getting list of users for county parish holding");

        pagedEntities.ShouldSatisfyAllConditions(
            (x) => x.Items.Count.ShouldBe(1),
            (x) => x.PageSize.ShouldBe(pageSize),
            (x) => x.PageNumber.ShouldBe(pageNumber),
            (x) => x.TotalCount.ShouldBe(3),
            (x) => x.TotalPages.ShouldBe(2));

        var firstItem = pagedEntities.Items[0];

        firstItem.ShouldSatisfyAllConditions(
            (x) => x.ApplicationId.ShouldBe(new Guid("112788f5-4cb5-4acc-a3f5-d8b2b0e20945")),
            (x) => x.RoleId.ShouldBe(new Guid("0c15ba2f-b4ba-406a-a0ae-213de64600a9")),
            (x) => x.UserAccountId.ShouldBe(new Guid("42bde7a0-9efe-402a-a7c3-9161be7b00ba")),
            (x) => x.CreatedAt.ShouldBe(DateTime.Parse("2026-03-01").ToUniversalTime()),
            (x) => x.CreatedById.ShouldBe(AdminUserId),
            (x) => x.DeletedAt.ShouldBeNull(),
            (x) => x.DeletedById.ShouldBeNull());
    }

    [Fact]
    [Description("Should get page one of cph users in a different cph ascending order with ordering and filters applied")]
    public async Task ShouldGetPageOneCphUsersInDifferentCphInAscendingOrderWithOrderingAndFiltersApplied()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphAssigneesRepository>>();
        var repository = new CphAssigneesRepository(ReadOnlyContext, logger);

        var cphId = new Guid("204459b1-3a07-4e65-9122-91c1699e3d3f");
        const int pageNumber = 1;
        const int pageSize = 2;
        const bool descendingOrder = false;

        Expression<Func<CountyParishHoldings, bool>> primaryFilter = cph => cph.Id == cphId;
        Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> associationFilter = cphUser => cphUser.DeletedAt == null;
        Expression<Func<ApplicationUserAccountHoldingAssignments, string>> orderBy = cphUser => cphUser.UserAccount.DisplayName;

        // Act
        var pagedEntities = await repository.GetPaged(primaryFilter, associationFilter, pageNumber, pageSize, orderBy, descendingOrder, TestContext.Current.CancellationToken);

        // Assert
        logger.Received(1).Log(LogLevel.Information, "Getting list of users for county parish holding");

        pagedEntities.ShouldSatisfyAllConditions(
            (x) => x.Items.Count.ShouldBe(2),
            (x) => x.PageSize.ShouldBe(pageSize),
            (x) => x.PageNumber.ShouldBe(pageNumber),
            (x) => x.TotalCount.ShouldBe(2),
            (x) => x.TotalPages.ShouldBe(1));

        var firstItem = pagedEntities.Items[0];
        var secondItem = pagedEntities.Items[1];

        firstItem.ShouldSatisfyAllConditions(
            (x) => x.ApplicationId.ShouldBe(new Guid("112788f5-4cb5-4acc-a3f5-d8b2b0e20945")),
            (x) => x.RoleId.ShouldBe(new Guid("0c15ba2f-b4ba-406a-a0ae-213de64600a9")),
            (x) => x.UserAccountId.ShouldBe(new Guid("42bde7a0-9efe-402a-a7c3-9161be7b00ba")),
            (x) => x.CreatedAt.ShouldBe(DateTime.Parse("2026-03-01").ToUniversalTime()),
            (x) => x.CreatedById.ShouldBe(AdminUserId),
            (x) => x.DeletedAt.ShouldBeNull(),
            (x) => x.DeletedById.ShouldBeNull());

        secondItem.ShouldSatisfyAllConditions(
            (x) => x.ApplicationId.ShouldBe(new Guid("112788f5-4cb5-4acc-a3f5-d8b2b0e20945")),
            (x) => x.RoleId.ShouldBe(new Guid("817647b3-d5d2-45e9-8833-df36d8264102")),
            (x) => x.UserAccountId.ShouldBe(new Guid("1e21b685-2247-4d96-bf39-f7dc30f356c2")),
            (x) => x.CreatedAt.ShouldBe(DateTime.Parse("2026-03-02").ToUniversalTime()),
            (x) => x.CreatedById.ShouldBe(AdminUserId),
            (x) => x.DeletedAt.ShouldBeNull(),
            (x) => x.DeletedById.ShouldBeNull());
    }

    [Fact]
    [Description("Should get page one of cph users in a different cph descending order with ordering and filters applied")]
    public async Task ShouldGetPageOneCphUsersInDifferentCphInDescendingOrderWithOrderingAndFiltersApplied()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphAssigneesRepository>>();
        var repository = new CphAssigneesRepository(ReadOnlyContext, logger);

        var cphId = new Guid("204459b1-3a07-4e65-9122-91c1699e3d3f");
        const int pageNumber = 1;
        const int pageSize = 2;
        const bool descendingOrder = true;

        Expression<Func<CountyParishHoldings, bool>> primaryFilter = cph => cph.Id == cphId;
        Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> associationFilter = cphUser => cphUser.DeletedAt == null;
        Expression<Func<ApplicationUserAccountHoldingAssignments, string>> orderBy = cphUser => cphUser.UserAccount.DisplayName;

        // Act
        var pagedEntities = await repository.GetPaged(primaryFilter, associationFilter, pageNumber, pageSize, orderBy, descendingOrder, TestContext.Current.CancellationToken);

        // Assert
        logger.Received(1).Log(LogLevel.Information, "Getting list of users for county parish holding");

        pagedEntities.ShouldSatisfyAllConditions(
            (x) => x.Items.Count.ShouldBe(2),
            (x) => x.PageSize.ShouldBe(pageSize),
            (x) => x.PageNumber.ShouldBe(pageNumber),
            (x) => x.TotalCount.ShouldBe(2),
            (x) => x.TotalPages.ShouldBe(1));

        var firstItem = pagedEntities.Items[0];
        var secondItem = pagedEntities.Items[1];

        firstItem.ShouldSatisfyAllConditions(
            (x) => x.ApplicationId.ShouldBe(new Guid("112788f5-4cb5-4acc-a3f5-d8b2b0e20945")),
            (x) => x.RoleId.ShouldBe(new Guid("817647b3-d5d2-45e9-8833-df36d8264102")),
            (x) => x.UserAccountId.ShouldBe(new Guid("1e21b685-2247-4d96-bf39-f7dc30f356c2")),
            (x) => x.CreatedAt.ShouldBe(DateTime.Parse("2026-03-02").ToUniversalTime()),
            (x) => x.CreatedById.ShouldBe(AdminUserId),
            (x) => x.DeletedAt.ShouldBeNull(),
            (x) => x.DeletedById.ShouldBeNull());

        secondItem.ShouldSatisfyAllConditions(
            (x) => x.ApplicationId.ShouldBe(new Guid("112788f5-4cb5-4acc-a3f5-d8b2b0e20945")),
            (x) => x.RoleId.ShouldBe(new Guid("0c15ba2f-b4ba-406a-a0ae-213de64600a9")),
            (x) => x.UserAccountId.ShouldBe(new Guid("42bde7a0-9efe-402a-a7c3-9161be7b00ba")),
            (x) => x.CreatedAt.ShouldBe(DateTime.Parse("2026-03-01").ToUniversalTime()),
            (x) => x.CreatedById.ShouldBe(AdminUserId),
            (x) => x.DeletedAt.ShouldBeNull(),
            (x) => x.DeletedById.ShouldBeNull());
    }
}

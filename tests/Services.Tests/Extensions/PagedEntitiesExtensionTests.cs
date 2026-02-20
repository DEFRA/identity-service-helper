// <copyright file="PagedEntitiesExtensionTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Extensions;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common;
using Defra.Identity.Responses.Cphs;

public class PagedEntitiesExtensionTests
{
    [Fact]
    [Description("Should map paged entities to paged results")]
    public void ShouldMapPagedEntitiesToPagedResults()
    {
        // Arrange
        var pagedEntities = new PagedEntities<CountyParishHoldings>(
            [
                new CountyParishHoldings
                {
                    Id = new Guid("34b4e480-8bd4-438e-b279-d0398a65a3a5"), Identifier = $"44/100/0001", ExpiredAt = DateTime.Parse("2026-02-12").ToUniversalTime(),
                },
                new CountyParishHoldings
                {
                    Id = new Guid("9928bbc3-ddd4-488a-a620-7850bcc653d7"), Identifier = $"44/100/0002", ExpiredAt = null,
                },
            ],
            6,
            3,
            1,
            2);

        // Act
        var pagedResults = pagedEntities.ToPagedResults(
            (entity) =>
                new Cph()
                {
                    Id = entity.Id, CphNumber = entity.Identifier, Expired = entity.ExpiredAt != null, ExpiredAt = entity.ExpiredAt,
                });

        // Assert
        pagedResults.ShouldSatisfyAllConditions(
            (x) => x.Items.Count().ShouldBe(2),
            (x) => x.PageNumber.ShouldBe(1),
            (x) => x.PageSize.ShouldBe(2),
            (x) => x.TotalCount.ShouldBe(6),
            (x) => x.TotalPages.ShouldBe(3));

        var pagedResultItems = pagedResults.Items.ToList();

        var firstItem = pagedResultItems[0];
        var secondItem = pagedResultItems[1];

        firstItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("34b4e480-8bd4-438e-b279-d0398a65a3a5")),
            (x) => x.CphNumber.ShouldBe("44/100/0001"),
            (x) => x.Expired.ShouldBe(true),
            (x) => x.ExpiredAt.ShouldBe(DateTime.Parse("2026-02-12").ToUniversalTime()));

        secondItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("9928bbc3-ddd4-488a-a620-7850bcc653d7")),
            (x) => x.CphNumber.ShouldBe("44/100/0002"),
            (x) => x.Expired.ShouldBe(false),
            (x) => x.ExpiredAt.ShouldBeNull());
    }
}

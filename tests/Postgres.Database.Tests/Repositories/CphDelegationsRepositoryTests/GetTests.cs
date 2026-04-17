// <copyright file="GetTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.CphDelegationsRepositoryTests;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Delegations;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

public class GetTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Should get a single delegation")]
    public async Task ShouldGetSingleDelegation()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphDelegationsRepository>>();
        var repository = new CphDelegationsRepository(Context, ReadOnlyContext, logger);

        var id = Guid.NewGuid();
        var adminUser = Context.UserAccounts.First();
        var cphId = new Guid("4435a146-d0ac-4260-8a27-c550e0ed9563");
        var delegatingUserId = new Guid("0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1");
        var delegatedUserId = new Guid("42bde7a0-9efe-402a-a7c3-9161be7b00ba");
        var delegatedUserRoleId = new Guid("0c15ba2f-b4ba-406a-a0ae-213de64600a9");
        const string delegatedUserEmail = "test1@test.com";
        var createdAt = DateTime.Parse("2026-03-01 00:00:00").ToUniversalTime();

        var delegation = new CountyParishHoldingDelegations
        {
            Id = id,
            CountyParishHoldingId = cphId,
            DelegatingUserId = delegatingUserId,
            DelegatedUserId = delegatedUserId,
            DelegatedUserRoleId = delegatedUserRoleId,
            DelegatedUserEmail = delegatedUserEmail,
            InvitationToken = string.Empty,
            CreatedById = adminUser.Id,
            CreatedAt = createdAt,
        };

        await Context.CountyParishHoldingDelegations.AddAsync(delegation, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await repository.GetSingle(
            x => x.Id == id,
            TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();

        result.ShouldSatisfyAllConditions(
            x => x.CountyParishHoldingId.ShouldBe(cphId),
            x => x.DelegatingUserId.ShouldBe(delegatingUserId),
            x => x.DelegatedUserId.ShouldBe(delegatedUserId),
            x => x.DelegatedUserRoleId.ShouldBe(delegatedUserRoleId),
            x => x.DelegatedUserEmail.ShouldBe(delegatedUserEmail),
            x => x.InvitationToken.ShouldBeNullOrWhiteSpace(),
            x => x.CreatedById.ShouldBe(adminUser.Id),
            x => x.CreatedAt.ShouldBeEquivalentTo(createdAt));
    }

    [Fact]
    [Description("Should get a list of delegations")]
    public async Task ShouldGetListDelegations()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphDelegationsRepository>>();
        var repository = new CphDelegationsRepository(Context, ReadOnlyContext, logger);

        var adminUser = Context.UserAccounts.First(x => x.Id == new Guid("cd91b1e0-bae4-4cee-becf-3529cc557311"));
        var createdAt = DateTime.Parse("2026-04-14 00:00:00.000").ToUniversalTime();

        var delegations = new List<CountyParishHoldingDelegations>
        {
            new()
            {
                Id = new Guid("3aaaf35e-4fa5-4e99-9721-7dc3bc1c9d7a"),
                CountyParishHoldingId = new Guid("4435a146-d0ac-4260-8a27-c550e0ed9563"),
                DelegatingUserId = new Guid("0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1"),
                DelegatedUserId = new Guid("42bde7a0-9efe-402a-a7c3-9161be7b00ba"),
                DelegatedUserRoleId = new Guid("0c15ba2f-b4ba-406a-a0ae-213de64600a9"),
                DelegatedUserEmail = "test1@test.com",
                InvitationToken = string.Empty,
                CreatedById = adminUser.Id,
                CreatedAt = createdAt,
            },
            new()
            {
                Id = new Guid("e4410973-af3f-4cbc-a234-69e8653da748"),
                CountyParishHoldingId = new Guid("204459b1-3a07-4e65-9122-91c1699e3d3f"),
                DelegatingUserId = new Guid("1e21b685-2247-4d96-bf39-f7dc30f356c2"),
                DelegatedUserId = new Guid("83bf35f9-fd59-4c8a-b70a-7d95a1aab2b6"),
                DelegatedUserRoleId = new Guid("817647b3-d5d2-45e9-8833-df36d8264102"),
                DelegatedUserEmail = "test3@test.com",
                InvitationToken = string.Empty,
                CreatedById = adminUser.Id,
                CreatedAt = createdAt,
            },
        };

        await Context.CountyParishHoldingDelegations.AddRangeAsync(delegations, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var results = await repository.GetList(x => true, TestContext.Current.CancellationToken);

        // Assert
        results.ShouldNotBeNull();
        results.Count.ShouldBe(6);

        var firstDelegation = results.First();
        var secondDelegation = results.Last();

        firstDelegation.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x.Id.ShouldBe(new Guid("dd000004-0000-4000-8000-000000000004")),
            x => x.CountyParishHoldingId.ShouldBe(new Guid("ab820005-0000-4000-8000-000000000005")),
            x => x.DelegatingUserId.ShouldBe(new Guid("cd91b1e0-bae4-4cee-becf-3529cc557311")),
            x => x.DelegatedUserId.ShouldBe(new Guid("a17a772c-604e-495d-950e-3dbee2ba6e98")),
            x => x.DelegatedUserRoleId.ShouldBe(new Guid("0c15ba2f-b4ba-406a-a0ae-213de64600a9")),
            x => x.DelegatedUserEmail.ShouldBe("max.bladen-clark@esynergy.co.uk"),
            x => x.InvitationToken.ShouldBe("0000000000000000000000000000000000000000000000000000000000000004"),
            x => x.CreatedById.ShouldBe(adminUser.Id),
            x => x.CreatedAt.ShouldBe(DateTime.Parse("2026-04-16 00:00:00.000")));

        secondDelegation.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x.Id.ShouldBe(new Guid("e4410973-af3f-4cbc-a234-69e8653da748")),
            x => x.CountyParishHoldingId.ShouldBe(new Guid("204459b1-3a07-4e65-9122-91c1699e3d3f")),
            x => x.DelegatingUserId.ShouldBe(new Guid("1e21b685-2247-4d96-bf39-f7dc30f356c2")),
            x => x.DelegatedUserId.ShouldBe(new Guid("83bf35f9-fd59-4c8a-b70a-7d95a1aab2b6")),
            x => x.DelegatedUserRoleId.ShouldBe(new Guid("817647b3-d5d2-45e9-8833-df36d8264102")),
            x => x.DelegatedUserEmail.ShouldBe("test3@test.com"),
            x => x.InvitationToken.ShouldBeNullOrWhiteSpace(),
            x => x.CreatedById.ShouldBe(adminUser.Id),
            x => x.CreatedAt.ShouldBe(DateTime.Parse("2026-04-13 23:00:00.000")));
    }
}

// <copyright file="GetTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.CphAssignmentsRepositoryTests;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Assignments;
using Defra.Identity.Test.Utilities.Assertions;
using Microsoft.Extensions.Logging;

public class GetTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Should get a list of cph assignment")]
    public async Task ShouldGetListCphAssignments()
    {
        // Arrange
        var logger = DefraLoggerExtensions.CreateNSubstituteLogger<CphAssignmentsRepository>();
        var repository = new CphAssignmentsRepository(ReadOnlyContext, logger);

        var adminUser = Context.UserAccounts.First();
        var testUser1 = Context.UserAccounts.First(x => x.EmailAddress == "test1@test.com");
        var testUser2 = Context.UserAccounts.First(x => x.EmailAddress == "test2@test.com");
        var role1 = Context.Roles.First(x => x.Name == "test-role-1");
        var role2 = Context.Roles.First(x => x.Name == "test-role-2");

        var countyParishHolding1 = Context.CountyParishHoldings.First(x => x.Identifier == "44/000/0001");
        var countyParishHolding2 = Context.CountyParishHoldings.First(x => x.Identifier == "44/000/0002");

        var roles = new List<UserAccountCountyParishHoldingAssignments>
        {
            new()
            {
                CountyParishHoldingId = countyParishHolding1.Id,
                UserAccountId = testUser1.Id,
                RoleId = role1.Id,
                CreatedById = adminUser.Id,
                CreatedAt = DateTime.UtcNow.AddDays(2),
            },
            new()
            {
                CountyParishHoldingId = countyParishHolding2.Id,
                UserAccountId = testUser2.Id,
                RoleId = role2.Id,
                CreatedById = adminUser.Id,
                CreatedAt = DateTime.UtcNow.AddDays(2),
            },
        };

        await Context.UserAccountCountyParishHoldingAssignments.AddRangeAsync(
            roles,
            TestContext.Current.CancellationToken);

        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var results =
            (await repository.GetList(
                x => x.CreatedAt > DateTime.UtcNow,
                TestContext.Current.CancellationToken))
            .OrderBy(x => x.CountyParishHolding.Identifier)
            .ToList();

        // Assert
        results.ShouldNotBeNull();
        results.Count.ShouldBe(2);

        results[0].ShouldSatisfyAllConditions(x =>
        {
            x.CountyParishHoldingId.ShouldBe(countyParishHolding1.Id);
            x.UserAccountId.ShouldBe(testUser1.Id);
            x.RoleId.ShouldBe(role1.Id);
            x.CreatedById.ShouldBe(adminUser.Id);
            x.CreatedAt.ShouldBeCloseToUtcNowAddDays(2);
        });

        results[1].ShouldSatisfyAllConditions(x =>
        {
            x.CountyParishHoldingId.ShouldBe(countyParishHolding2.Id);
            x.UserAccountId.ShouldBe(testUser2.Id);
            x.RoleId.ShouldBe(role2.Id);
            x.CreatedById.ShouldBe(adminUser.Id);
            x.CreatedAt.ShouldBeCloseToUtcNowAddDays(2);
        });

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Getting list of county parish holding assignments");
    }
}

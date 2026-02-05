// <copyright file="UsersRepositoryTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories;
using Defra.Identity.Repositories.Users;
using Microsoft.EntityFrameworkCore;
using Shouldly;

public class UpdateTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    private const string AdminEmailAddress = "test@test.com";

    [Fact]
    [Description("Should update an existing user account")]
    public async Task ShouldUpdateUserAccount()
    {
        // Arrange
        var repository = new UsersRepository(Context);
        var adminUser = await repository.GetSingle(
            x =>
            x.EmailAddress.Equals(AdminEmailAddress),
            TestContext.Current.CancellationToken);

        adminUser.ShouldNotBeNull();
        var user = new UserAccount
        {
            DisplayName = "Test User",
            FirstName = "Test",
            LastName = "User",
            EmailAddress = "test2@test.com",
            CreatedBy = adminUser.Id,
        };
        await Context.Users.AddAsync(user, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        user.DisplayName = "Updated Name";

        // Act
        user.Status = new StatusType { Name = "InActive", Description = "Updated Description" };
        var result = await repository.Update(user, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldSatisfyAllConditions(
            x => x.DisplayName.ShouldBe("Updated Name"),
            x => x.Status.Name.ShouldBe("InActive"));

        var savedUser = await Context.Users.Include(x => x.Status).FirstAsync(x => x.EmailAddress.Equals(user.EmailAddress), TestContext.Current.CancellationToken);

        savedUser.ShouldSatisfyAllConditions(
            x => x.DisplayName.ShouldBe("Updated Name"),
            x => x.Status.Name.ShouldBe("InActive"));

        var statusInDb = await Context.StatusTypes.FirstAsync(s => s.Name == "InActive", TestContext.Current.CancellationToken);
        statusInDb.Description.ShouldNotBe("Updated Description");
        statusInDb.Description.ShouldBe(string.Empty);

        savedUser.ShouldNotBeNull();
        savedUser.DisplayName.ShouldBe("Updated Name");
    }
}

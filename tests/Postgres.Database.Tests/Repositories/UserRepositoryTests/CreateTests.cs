namespace Defra.Identity.Postgres.Database.Tests.Repositories;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories;

public class CreateTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    private const string AdminEmailAddress = "test@test.com";

    [Fact]
    [Description("Should create a new user account")]
    public async Task ShouldCreateUserAccount()
    {
        // Arrange
        var repository = new UsersRepository(Context);

        var adminEmail = AdminEmailAddress;

        var adminUser = await repository.GetSingle(
            x =>
                x.EmailAddress == adminEmail,
            TestContext.Current.CancellationToken);

        adminUser.ShouldNotBeNull("Seeded admin user was not found; check test data initialization.");

        var newUser = new UserAccount
        {
            DisplayName = "Test User",
            FirstName = "Test",
            LastName = "User",
            EmailAddress = "test1@test.com",
            CreatedBy = adminUser.Id,
        };

        // Act
        var createdUser = await repository.Create(newUser, TestContext.Current.CancellationToken);

        // Assert
        createdUser.ShouldSatisfyAllConditions(
            x => x.DisplayName.ShouldBe("Test User"),
            x => x.FirstName.ShouldBe("Test"),
            x => x.LastName.ShouldBe("User"),
            x => x.StatusTypeId.ShouldBe(1));
    }
}

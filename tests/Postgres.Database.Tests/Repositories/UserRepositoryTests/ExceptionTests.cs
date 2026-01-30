namespace Defra.Identity.Postgres.Database.Tests.Repositories;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories;
using Microsoft.EntityFrameworkCore;

public class ExceptionTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    private const string AdminEmailAddress = "test@test.com";

    [Fact]
    [Trait("Category", "Integration")]
    [Description("Should throw duplicate exception")]
    public async Task ShouldThrowDuplicateException()
    {
        // Arrange
        var repository = new UsersRepository(Context);

        var adminUser = await repository.GetSingle(x => x.EmailAddress == AdminEmailAddress, TestContext.Current.CancellationToken);
        adminUser.ShouldNotBeNull();

        var duplicateUser = new UserAccount
        {
            DisplayName = "Dup User",
            FirstName = "Dup",
            LastName = "User",
            EmailAddress = AdminEmailAddress,   // <-- violates unique constraint om email address field
            CreatedBy = adminUser.Id,
        };

        // Act
        Func<Task> act = async () => await repository.Create(duplicateUser);

        // Assert (Shouldly)
        var ex = await act.ShouldThrowAsync<DbUpdateException>();
        ex.InnerException.ShouldNotBeNull(); // often contains provider-specific details
    }
}

namespace Livestock.Auth.Database.Tests.Applications;

using System.ComponentModel;
using Entities;
using Microsoft.EntityFrameworkCore;

public class ApplicationTests(PostgreContainerFixture fixture): BaseTests(fixture)
{
    [Fact, Description("Add a new application and check it's been added")]
    public async Task ShouldAddApplication()
    {
        var clientId = Guid.NewGuid();
        var app = new Application
        {
            ClientId = clientId,
            TenantName = "Test Tenant",
            Name = "Test Application",
            Description = "Test Application Description",
            Status = "Active"
        };

        await Context.Applications.AddAsync(app, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await Context.Applications.SingleAsync(x => x.ClientId == clientId, TestContext.Current.CancellationToken);

        result.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x.ShouldBeOfType<Application>(),
            x => x.CreatedAt.ShouldNotBe(default)
            );

    }

}

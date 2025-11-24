// <copyright file="ApplicationTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgre.Database.Tests.Applications;

using System.ComponentModel;
using Defra.Identity.Postgre.Database.Entities;
using Defra.Identity.Postgre.Database.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;

public class ApplicationTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Add a new application and check it's been added")]
    public async Task ShouldAddApplication()
    {
        var clientId = Guid.NewGuid();
        var app = new Application
        {
            ClientId = clientId,
            TenantName = "Test Tenant",
            Name = "Test Application",
            Description = "Test Application Description",
            Status = "Active",
        };

        await Context.Applications.AddAsync(app, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await Context.Applications.SingleAsync(x => x.ClientId == clientId, TestContext.Current.CancellationToken);

        result.ShouldSatisfyAllConditions(
            application => application.ShouldNotBeNull(),
            application => application.ShouldBeOfType<Application>(),
            application => application.CreatedAt.ShouldNotBe(default),
            application => application.UpdatedAt.ShouldBe(default),
            application => application.Name.ShouldBe(app.Name),
            application => application.Description.ShouldBe(app.Description),
            application => application.Status.ShouldBe(app.Status),
            application => application.TenantName.ShouldBe(app.TenantName),
            application => application.ClientId.ShouldBe(app.ClientId),
            application => application.Id.ShouldNotBe(Guid.Empty));
    }

    [Fact]
    [Description("Same as the above test however this time we update the record to ensure the updated at timestamp is set")]
    public async Task ShouldUpdateApplication()
    {
        var clientId = Guid.NewGuid();
        var app = new Application
        {
            ClientId = clientId,
            TenantName = "Test Tenant",
            Name = "Test Application",
            Description = "Test Application Description",
            Status = "Active",
        };
        await Context.Applications.AddAsync(app, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await Context.Applications.SingleAsync(x => x.ClientId == clientId, TestContext.Current.CancellationToken);

        result.ShouldSatisfyAllConditions(
            application => application.ShouldNotBeNull(),
            application => application.ShouldBeOfType<Application>(),
            application => application.CreatedAt.ShouldNotBe(default),
            application => application.UpdatedAt.ShouldBe(default),
            application => application.Name.ShouldBe(app.Name),
            application => application.Description.ShouldBe(app.Description),
            application => application.Status.ShouldBe(app.Status),
            application => application.TenantName.ShouldBe(app.TenantName),
            application => application.ClientId.ShouldBe(app.ClientId),
            application => application.Id.ShouldNotBe(Guid.Empty));

        var updated = await Context.Applications.SingleAsync(x => x.ClientId.Equals(clientId), TestContext.Current.CancellationToken);
        updated.Name = "Updated Name";

        Context.Applications.Update(updated);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var checkUpdated = await Context.Applications.SingleAsync(x => x.ClientId.Equals(clientId), TestContext.Current.CancellationToken);
        checkUpdated.ShouldSatisfyAllConditions(
            application => application.UpdatedAt.ShouldNotBe(default),
            application => application.Name.ShouldBe("Updated Name"));
    }
}

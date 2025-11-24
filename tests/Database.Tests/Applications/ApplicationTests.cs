// <copyright file="ApplicationTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Database.Tests.Applications;

using System.ComponentModel;
using Defra.Identity.Database.Entities;
using Defra.Identity.Database.Tests;
using Defra.Identity.Database.Tests.Fixtures;
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

        var result =
            await Context.Applications.SingleAsync(x => x.ClientId == clientId, TestContext.Current.CancellationToken);

        result.ShouldSatisfyAllConditions(
            application => application.ShouldNotBeNull(),
            application => application.ShouldBeOfType<Application>(),
            application => application.CreatedAt.ShouldNotBe(default),
            application => application.UpdatedAt.ShouldNotBe(default),
            application => application.Name.ShouldBe(app.Name),
            application => application.Description.ShouldBe(app.Description),
            application => application.Status.ShouldBe(app.Status),
            application => application.TenantName.ShouldBe(app.TenantName),
            application => application.ClientId.ShouldBe(app.ClientId),
            application => application.Id.ShouldNotBe(Guid.Empty));
    }
}

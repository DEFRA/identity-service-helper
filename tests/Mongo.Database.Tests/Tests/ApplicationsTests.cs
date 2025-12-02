// <copyright file="ApplicationsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Mongo.Database.Tests.Tests;

using System.ComponentModel;
using System.Net.Mime;
using Defra.Identity.Mongo.Database.Documents;
using Defra.Identity.Mongo.Database.Tests.Fixtures;
using MongoDB.Driver.Linq;

public class ApplicationsTests(MongoContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Add a new application and check it's been added")]
    public async Task ShouldAddApplication()
    {
        var clientId = Guid.NewGuid();
        var app = new Applications()
        {
            ClientId = clientId,
            TenantName = "Test Tenant",
            Name = "Test Application",
            Description = "Test Application Description",
            Status = "Active",
        };

        await Context.Applications.AddAsync(app, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = Context.Applications.FirstOrDefault(x => x.ClientId.Equals(clientId));

        result.ShouldSatisfyAllConditions(
            application => application.ShouldNotBeNull(),
            application => application.ShouldBeOfType<Applications>(),
            application => application?.Name.ShouldBe(app.Name),
            application => application?.Description.ShouldBe(app.Description),
            application => application?.Status.ShouldBe(app.Status),
            application => application?.TenantName.ShouldBe(app.TenantName),
            application => application?.ClientId.ShouldBe(app.ClientId),
            application => application?.Id.ShouldNotBe(Guid.Empty));
    }
}

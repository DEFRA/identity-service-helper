// <copyright file="ApplicationMapperTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Common.Mappers;

using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Services.Common.Mappers;

public class ApplicationMapperTests
{
    [Fact]
    public void ApplicationMapper_ReturnsValidInstance()
    {
        // Arrange
        var application = new Applications
        {
            ClientId = Guid.NewGuid(),
            Name = "Test name",
            TenantName = "Test tenant",
            Description = "Test Description",
            Scopes = "Scope1;Scope2;Scope3",
            Secret = "Test secret",
            RedirectUris = "https://example.com;https://example1.com;https://example2.com",
            DeletedByUser = new UserAccounts(),
            CreatedByUser = new UserAccounts(),
        };

        // Act
        var result = ApplicationMapper.MapApplicationEntityToApplication(application);

        // Assert
        result.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x.Id.ShouldBe(application.ClientId),
            x => x.Name.ShouldBe(application.Name),
            x => x.TenantName.ShouldBe(application.TenantName),
            x => x.Description.ShouldBe(application.Description),
            x => x.Secret.ShouldBe(application.Secret),
            x => x.Scopes.Count.ShouldBe(3),
            x => x.Scopes.ShouldContain("Scope1"),
            x => x.RedirectUris.Count.ShouldBe(3),
            x => x.RedirectUris.ShouldContain("https://example.com"));
    }
}
